using CollectionHub.DataManagement;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using CollectionHub.Domain.Converters;
using System.Reflection;

namespace CollectionHub.Services
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;

        public ItemService(ApplicationDbContext context) => _context = context;

        public async Task DeleteItem(long id)
        {
            var item = await _context.Items
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == id);

            _context.Tags.RemoveRange(item.Tags);
            _context.Items.Remove(item);

            await _context.SaveChangesAsync();
        }

        public async Task<List<List<string>>> GetCollectionItems(long collectionId, Dictionary<string, string> fieldNames)
        {
            var items = await _context.Items
                .Where(item => item.CollectionId == collectionId)
                .Include(item => item.Tags)
                .ToListAsync();

            var itemProperties = ToItemPropertyNames(fieldNames);

            var result = new List<List<string>>();

            foreach (var item in items)
            {
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

                result.Add(itemValues);
            }

            return result;
        }

        public async Task<long> CreateItem(string userName, IFormCollection formCollection)
        {
            var collectionId = ExtractCollectionId(formCollection);

            var fieldsWithValues = ConvertFormCollectionToDictionary(formCollection);

            await CheckIsUserHasCollection(userName, collectionId);

            var itemDbProperties = typeof(ItemDb).GetProperties();
            var newItem = new ItemDb();

            foreach (var item in fieldsWithValues)
            {
                var property = itemDbProperties.FirstOrDefault(p => p.Name == item.Key.ToItemDbProperty());

                if (item.Key == "Tags")
                {
                    newItem.Tags = ParseTags(item.Value);
                }
                else
                {
                    if (property != null)
                    {
                        var value = item.Value.ToDynamicType(property.PropertyType);
                        var valueRes = ChangeType(value, property.PropertyType);

                        property.SetValue(newItem, valueRes);
                    }
                }
            }

            newItem.CollectionId = collectionId;
            newItem.CreationDate = DateTimeOffset.Now;

            await _context.AddAsync(newItem);
            await _context.SaveChangesAsync();

            return collectionId;
        }

        private static string GetPropertyValueAsString(PropertyInfo propertyInfo, ItemDb item)
        {
            var value = propertyInfo.GetValue(item);

            if (propertyInfo.PropertyType == typeof(DateTimeOffset?) && value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset.Date.ToString("yyyy-MM-dd");
            }

            return value?.ToString() ?? "-";
        }

        private List<string> ToItemPropertyNames(Dictionary<string, string> collectionNames)
        {
            var result = new List<string>();

            foreach (var item in collectionNames.Keys)
            {
                result.Add(item.ToItemDbProperty());
            }

            return result;
        }

        private long ExtractCollectionId(IFormCollection formCollection)
        {
            if (formCollection.TryGetValue("id", out var idString) && long.TryParse(idString, out var collectionId))
            {
                return collectionId;
            }

            throw new FormatException();
        }

        private Dictionary<string, string> ConvertFormCollectionToDictionary(IFormCollection formCollection)
        {
            var fieldsWithValues = new Dictionary<string, string>();

            foreach (var key in formCollection.Keys)
            {
                fieldsWithValues[key] = formCollection[key];
            }

            return fieldsWithValues;
        }

        public static object ChangeType(object value, Type conversion)
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

        private List<TagDb> ParseTags(string tags)
        {
            char[] separators = { ',', ' ' };

            var tagArray = tags.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            return tagArray
                .Where(tag => !string.IsNullOrWhiteSpace(tag) && IsContainsLettersOrNumbers(tag))
                .Select(tag => new TagDb { Name = tag })
                .ToList();
        }

        private bool IsContainsLettersOrNumbers(string input) => input.Any(char.IsLetterOrDigit);

        public async Task CheckIsUserHasCollection(string userName, long collectionId)
        {
            var collection = await _context.Collections
                .Where(x => x.User.UserName == userName)
                .FirstOrDefaultAsync(x => x.Id == collectionId) ?? throw new AccessViolationException();
        }
    }
}
