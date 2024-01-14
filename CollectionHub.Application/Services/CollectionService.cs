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

        private readonly IItemService _itemService;

        public CollectionService(ApplicationDbContext context, UserManager<UserDb> userManager, IItemService itemService)
        {
            _context = context;
            _userManager = userManager;
            _itemService = itemService;
        }

        public async Task<CollectionViewModel> GetEmptyCollectionViewModel()
        {
            var categories = await GetAllCategories();

            return new CollectionViewModel
            {
                Categories = categories.ToSelectListItem()
            };
        }

        public async Task EditCollection(string userName, CollectionViewModel collectionViewModel)
        {
            var collectionDb = await _context.Collections
                .Where(x => x.User.UserName == userName)
                .FirstAsync(x => x.Id == collectionViewModel.Id);

            var categoryDb = await _context.Categories.FirstAsync(x => x.Name == collectionViewModel.Category);

            collectionDb.Name = collectionViewModel.Name;
            collectionDb.Description = collectionViewModel.Description;
            collectionDb.CategoryId = categoryDb.Id;
            collectionDb.ImageUrl = collectionViewModel.ImageUrl;

            await _context.SaveChangesAsync();
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
            var collection = await _context.Collections
                .Include(x => x.Category)
                .Where(x => x.User.UserName == userName)
                .FirstAsync(x => x.Id == id);

            var categories = await GetAllCategories();

            var nonNullFieldNames = GetNonNullFieldNames(collection);

            var items = await _itemService.GetCollectionItems(id, new Dictionary<string, string>(nonNullFieldNames));

            return collection.ToCollectionViewModel(nonNullFieldNames, items, categories);
        }

        public Dictionary<string, string> GetNonNullFieldNames(CollectionDb collection)
        {
            var propertyNames = StringConverter.GetCollectionFieldNames();
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
            var collection = await _context.Collections
                .Include(x => x.Items)
                .ThenInclude(x => x.Tags)
                .FirstAsync(x => x.Id == id);

            _context.Tags.RemoveRange(collection.Items.SelectMany(item => item.Tags));
            _context.RemoveRange(collection.Items);
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
    }
}
