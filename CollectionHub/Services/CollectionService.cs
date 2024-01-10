using CollectionHub.DataManagement;
using CollectionHub.Helpers;
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
            var user = await _userManager.FindByNameAsync(userName);
            var collection = await _context.Collections
                .FirstAsync(x => x.UserId == user.Id && x.Id == id);

            return await UpdateCollectionFieldName(type, collection, name);
        }

        public async Task<List<string>> GetAllCategories() => await _context.Categories.Select(x => x.Name).ToListAsync();

        public async Task<List<CollectionViewModel>> GetUserCollections(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var collections = await _context.Collections
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.CreationDate)
                .ToListAsync();
            return collections.ToCollectionViewModelList();
        }

        public async Task<CollectionViewModel> GetUserCollection(string userName, long id)
        {
            //get and set items here and headers
            var user = await _userManager.FindByNameAsync(userName);
            var collection = await _context.Collections
                .FirstAsync(x => x.UserId == user.Id && x.Id == id);

            var nonNullFieldNames = GetNonNullFieldNames(collection);

            return collection.ToCollectionViewModel(nonNullFieldNames);
        }

        public Dictionary<string, string> GetNonNullFieldNames(CollectionDb collection)
        {
            var propertyNames = GetPropertyNames();
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

        private CollectionDb CreateCollectionDbInstance(UserDb user, CollectionViewModel collection, CategoryDb category)
        {
            return new CollectionDb
            {
                UserId = user.Id,
                Name = collection.Name,
                Description = collection.Description,
                ImageUrl = collection.ImageUrl,
                CategoryId = category.Id,
                CreationDate = DateTimeOffset.Now
            };
        }

        private async Task<bool> UpdateCollectionFieldName(DataType type, CollectionDb collection, string name)
        {
            var propertyNames = type.ToPropertyNames();
            if (IsFieldExist(propertyNames, collection, name)) return false;
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

        private string[] GetPropertyNames()
        {
            return [
                "String1Name",
                "String2Name",
                "String3Name",
                "Int1Name",
                "Int2Name",
                "Int3Name",
                "Text1Name",
                "Text2Name",
                "Text3Name",
                "Bool1Name",
                "Bool2Name",
                "Bool3Name",
                "Date1Name",
                "Date2Name",
                "Date3Name"];
        }
    }
}
