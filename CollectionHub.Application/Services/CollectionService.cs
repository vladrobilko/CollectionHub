using CollectionHub.DataManagement;
using CollectionHub.Domain.Converters;
using CollectionHub.Models.Enums;
using CollectionHub.Models.ViewModels;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CollectionHub.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<UserDb> _userManager;

        public CollectionService(ApplicationDbContext context, UserManager<UserDb> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> AddCollectionItemField(string userName, long id, DataType type, string name)
        {
            var collection = await _context.Collections
                .Where(x => x.User.UserName == userName)
                .FirstAsync(x => x.Id == id);

            return await UpdateCollectionFieldName(type, collection, name);
        }

        public async Task<List<string>> GetAllCategories() => await _context.Categories.Select(x => x.Name).ToListAsync();

        public async Task<List<CollectionViewModel>> GetUserCollections(string userName)
        {
            var collections = await _context.Collections
                .Where(x => x.User.UserName == userName)
                .OrderByDescending(x => x.CreationDate)
                .ToListAsync();

            return collections.ToCollectionViewModelList();
        }

        public async Task<CollectionViewModel> GetUserCollection(string userName, long id)
        {
            //get and set items here and headers
            var collection = await _context.Collections
                .Where(x => x.User.UserName == userName)
                .FirstAsync(x => x.Id == id);

            var nonNullFieldNames = GetNonNullFieldNames(collection);

            return collection.ToCollectionViewModel(nonNullFieldNames);
        }

        public Dictionary<string, string> GetNonNullFieldNames(CollectionDb collection)
        {
            var propertyNames = GetCollectionPropertyNames();
            var propertyInfos = collection.GetType().GetProperties();

            return propertyInfos
                .Where(property =>
                    propertyNames.Contains(property.Name) &&
                    property.PropertyType == typeof(string) &&
                    property.GetValue(collection) != null)
                .ToDictionary(property => property.Name, property => (string)property.GetValue(collection)!);
        }

        public async Task CreateCollection(CollectionViewModel collection, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var category = await _context.Categories.FirstAsync(x => x.Name == collection.Category);

            await _context.AddAsync(CreateCollectionDbInstance(user, collection, category));
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCollection(long id)
        {
            var collection = await _context.Collections.FirstAsync(x => x.Id == id);
            _context.Remove(collection);
            await _context.SaveChangesAsync();
        }

        private CollectionDb CreateCollectionDbInstance(UserDb user, CollectionViewModel collection, CategoryDb category) =>        
             new CollectionDb
            {
                UserId = user.Id,
                Name = collection.Name,
                Description = collection.Description,
                ImageUrl = collection.ImageUrl,
                CategoryId = category.Id,
                CreationDate = DateTimeOffset.Now
            };        

        private async Task<bool> UpdateCollectionFieldName(DataType type, CollectionDb collection, string name)
        {
            var propertyNames = type.ToCollectionProperty();
            if (IsFieldExist(propertyNames, collection, name))
            {
                return false;
            }
            foreach (var propertyName in propertyNames)
            {
                var property = collection.GetType().GetProperty(propertyName);
                var value = (string)property.GetValue(collection);

                if (value == null)
                {
                    property.SetValue(collection, name);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        private bool IsFieldExist(string[] propertyNames, CollectionDb collection, string name)
        {
            return propertyNames
                .Select(propertyName => (string)collection.GetType().GetProperty(propertyName).GetValue(collection))
                .ToList().Contains(name);
        }

        private string[] GetCollectionPropertyNames()
        {
            return [
                nameof(CollectionDb.String1Name),
                nameof(CollectionDb.String2Name),
                nameof(CollectionDb.String3Name),
                nameof(CollectionDb.Int1Name),
                nameof(CollectionDb.Int2Name),
                nameof(CollectionDb.Int3Name),
                nameof(CollectionDb.Text1Name),
                nameof(CollectionDb.Text2Name),
                nameof(CollectionDb.Text3Name),
                nameof(CollectionDb.Bool1Name),
                nameof(CollectionDb.Bool2Name),
                nameof(CollectionDb.Bool3Name),
                nameof(CollectionDb.Date1Name),
                nameof(CollectionDb.Date2Name),
                nameof(CollectionDb.Date3Name)];
        }
    }
}
