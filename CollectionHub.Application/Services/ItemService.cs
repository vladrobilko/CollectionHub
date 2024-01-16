using CollectionHub.DataManagement;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using CollectionHub.Domain.Converters;
using System.Reflection;
using CollectionHub.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace CollectionHub.Services
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<UserDb> _userManager;

        public ItemService(ApplicationDbContext context, UserManager<UserDb> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task ProcessLikeItem(string userName, long itemId)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var existingLike = await _context.Likes
                .Where(like => like.UserId == user.Id && like.ItemId == itemId)
                .FirstOrDefaultAsync();

            if (existingLike == null)
            {
                var newLike = new LikeDb
                {
                    UserId = user.Id, 
                    ItemId = itemId
                };

                _context.Likes.Add(newLike);
            }
            else
            {
                _context.Likes.Remove(existingLike);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<ItemViewModel>> GetRecentlyAddedItemsForRead()
        {
            var items = await _context.Items
             .AsNoTracking()
             .Include(x => x.Collection)
             .Include(x => x.Collection.User)
             .OrderByDescending(x => x.CreationDate)
             .ToListAsync();

            return items.ToItemViewModelList();
        }

        public async Task<List<List<string>>> GetCollectionItems(long collectionId, Dictionary<string, string> fieldNames)
        {
            var items = await _context.Items
                .AsNoTracking()
                .Where(item => item.CollectionId == collectionId)
                .Include(item => item.Tags)
                .ToListAsync();

            var itemProperties = fieldNames.ToItemPropertyNames();

            var result = new List<List<string>>();

            foreach (var item in items)
            {
                //
                var itemValues = new List<string>
                {
                    item.Id.ToString(),
                    item.Name,
                    string.Join(", ", item.Tags.Select(tag => "#" + tag.Name))
                };

                foreach (var fieldName in itemProperties)
                {
                    var propertyInfo = typeof(ItemDb).GetProperty(fieldName);

                    if (propertyInfo != null)
                    {
                        var value = GetPropertyValueAsString(propertyInfo, item);
                        itemValues.Add(value);
                    }
                }
                //

                result.Add(itemValues);
            }

            return result;
        }

        public async Task<ItemViewModel> GetItem(long itemId, long collectionId)
        {
            var collection = await _context.Collections
                .AsNoTracking()
                .Include(x => x.Items)
                .ThenInclude(x => x.Tags)
                .Include(x => x.Items)
                .ThenInclude(x => x.Likes)
                .FirstAsync(x => x.Id == collectionId);

            var nonNullFieldNames = collection.GetNonNullStringFields();
            var itemProperties = nonNullFieldNames.ToDictionary(
                    kvp => kvp.Key.ToItemDbProperty(),
                    kvp => kvp.Value
                );

            var item = collection.Items.First(x => x.Id == itemId);

            var headersWithValues = SetItemToDictionary(itemProperties, item);

            return new ItemViewModel()
            {
                Id = itemId,
                CollectionId = collectionId,
                AllHeadersWithValues = headersWithValues,
                Likes = item.Likes.Count
            };
        }

        private Dictionary<string, Dictionary<string, string>> SetItemToDictionary(Dictionary<string,string> itemProperties, ItemDb item)
        {
            var result = new Dictionary<string, Dictionary<string, string>>
            {
                {
                    nameof(ItemDb.Name), new Dictionary<string, string>
                    {
                        { nameof(ItemDb.Name), item.Name }
                    }
                },
                {
                    nameof(ItemDb.Tags), new Dictionary<string, string>
                    {
                        { nameof(ItemDb.Tags), string.Join(", ", item.Tags.Select(tag => "#" + tag.Name)) }
                    }
                },
            };

            foreach (var fieldName in itemProperties.Keys)
            {
                var propertyInfo = typeof(ItemDb).GetProperty(fieldName);

                if (propertyInfo != null)
                {
                    var value = GetPropertyValueAsString(propertyInfo, item);
                    result.Add(fieldName, new Dictionary<string, string>
                    {
                        { itemProperties[fieldName], value }
                    });
                }
            }

            return result;
        }

        public async Task<long> EditItem(string userName, IFormCollection formCollection)
        {
            var itemId = formCollection.ToId();
            var fieldsWithValues = formCollection.ToDictionary();

            var itemToUpdate = await _context.Items
                .Include(x => x.Tags)
                .FirstAsync(x => x.Id == itemId);

            var itemDbProperties = typeof(ItemDb).GetProperties();

            foreach (var item in fieldsWithValues)
            {
                var property = itemDbProperties.FirstOrDefault(p => p.Name == item.Key);

                if (item.Key == nameof(ItemDb.Tags))
                {
                    UpdateTags(itemToUpdate, item.Value);
                }
                else
                {
                    SetProperty(item, property, itemToUpdate);
                }
            }

            await _context.SaveChangesAsync();

            return itemToUpdate.CollectionId;
        }

        public async Task<long> CreateItem(string userName, IFormCollection formCollection)
        {
            var collectionId = formCollection.ToId();

            var fieldsWithValues = formCollection.ToDictionary();

            await CheckIsUserHasCollection(userName, collectionId);

            var itemDbProperties = typeof(ItemDb).GetProperties();
            var newItem = new ItemDb();

            foreach (var item in fieldsWithValues)
            {
                var property = itemDbProperties.FirstOrDefault(p => p.Name == item.Key.ToItemDbProperty());

                if (item.Key == nameof(ItemDb.Tags))
                {
                    newItem.Tags = item.Value.ToTags();
                }
                else
                {
                    SetProperty(item, property, newItem);
                }
            }

            newItem.CollectionId = collectionId;
            newItem.CreationDate = DateTimeOffset.Now;

            await _context.AddAsync(newItem);
            await _context.SaveChangesAsync();

            return collectionId;
        }



        private void SetProperty(KeyValuePair<string, string> item, PropertyInfo property, ItemDb itemToSet)
        {
            if (property != null)
            {
                var value = item.Value.ToDynamicType(property.PropertyType);
                var valueRes = ChangeType(value, property.PropertyType);
                property.SetValue(itemToSet, valueRes);
            }
        }

        private void UpdateTags(ItemDb itemToUpdate, string tagsValue)
        {
            var newTags = tagsValue.ToTags();

            var existingTags = itemToUpdate.Tags.ToList();

            foreach (var newTag in newTags)
            {
                if (!existingTags.Any(existingTag => existingTag.Name == newTag.Name))
                {
                    itemToUpdate.Tags.Add(newTag);
                }
            }
        }

        private string GetPropertyValueAsString(PropertyInfo propertyInfo, ItemDb item)
        {
            var value = propertyInfo.GetValue(item);

            if (propertyInfo.PropertyType == typeof(DateTimeOffset?) && value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset.Date.ToString("yyyy-MM-dd");
            }

            return value?.ToString() ?? "-";
        }

        private object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }


        private async Task CheckIsUserHasCollection(string userName, long collectionId)
        {
            var collection = await _context.Collections
                .Where(x => x.User.UserName == userName)
                .FirstOrDefaultAsync(x => x.Id == collectionId) ?? throw new AccessViolationException();
        }

        public async Task DeleteItem(long id)
        {
            var item = await _context.Items
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == id);

            _context.Tags.RemoveRange(item.Tags);
            _context.Items.Remove(item);

            await _context.SaveChangesAsync();
        }
    }
}
