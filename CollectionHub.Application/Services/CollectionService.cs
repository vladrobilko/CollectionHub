using CollectionHub.DataManagement;
using CollectionHub.Domain.Converters;
using CollectionHub.Domain.Interfaces;
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

        private readonly ICollectionFieldNameUpdater _fieldNameUpdater;
        
        private readonly IAlgoliaIntegration _algolia;

        public CollectionService(ApplicationDbContext context, UserManager<UserDb> userManager, IItemService itemService, ICollectionFieldNameUpdater fieldNameUpdater, IAlgoliaIntegration algolia)
        {
            _context = context;
            _userManager = userManager;
            _itemService = itemService;
            _fieldNameUpdater = fieldNameUpdater;
            _algolia = algolia;
        }

        public async Task CreateCollection(CollectionViewModel collection, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var category = await _context.Categories.FirstAsync(x => x.Name == collection.Category);

            await _context.AddAsync(CreateCollectionDbInstance(user, collection, category));
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CreateCollectionItemField(string userName, long id, DataType type, string name)
        {
            var collection = await _context.Collections
                .Where(x => x.User.UserName == userName)
                .FirstAsync(x => x.Id == id);

            var isUpdated = _fieldNameUpdater.TryUpdateCollectionFieldName(type, collection, name);

            if (isUpdated)
            {
                await _context.SaveChangesAsync();
            }

            return isUpdated;
        }

        public async Task<List<CollectionViewModel>> GetUserCollections(string userName)
        {
            var collections = await _context.Collections
                .AsNoTracking()
                .Where(x => x.User.UserName == userName)
                .OrderByDescending(x => x.CreationDate)
                .ToListAsync();

            return collections.ToCollectionViewModelList();
        }

        public async Task<CollectionViewModel> GetUserCollection(string userName, long id)
        {
            var collection = await _context.Collections
                .AsNoTracking()
                .Include(x => x.Category)
                .Where(x => x.User.UserName == userName)
                .FirstAsync(x => x.Id == id);

            var categories = await GetAllCategories();

            var items = await _itemService.GetCollectionItems(id, collection.GetNonNullStringFields());

            return collection.ToCollectionViewModel(items, categories);
        }

        public async Task<CollectionViewModel> GetEmptyCollectionViewModel()
        {
            var categories = await GetAllCategories();

            return new CollectionViewModel
            {
                Categories = categories.ToSelectListItem()
            };
        }

        public async Task<List<CollectionViewModel>> GetLargestCollections()
        {
            var collections = await _context.Collections
                .AsNoTracking()
                .OrderByDescending(x => x.Items.Count)
                .Take(5)
                .ToListAsync();

            return collections.ToCollectionViewModelList();
        }

        public async Task<CollectionViewModel> GetCollectionForRead(long id)
        {
            var collection = await _context.Collections
                .AsNoTracking()
                .Include(x => x.Category)
                .FirstAsync(x => x.Id == id);

            var categories = await GetAllCategories();

            var items = await _itemService.GetCollectionItems(id, collection.GetNonNullStringFields());

            return collection.ToCollectionViewModel(items, categories);
        }

        public async Task EditCollection(string userName, CollectionViewModel collectionViewModel)// change it 
        {
            var categoryDb = await _context.Categories.FirstAsync(x => x.Name == collectionViewModel.Category);

            var collectionDb = await _context.Collections
                .FirstAsync(x => x.Id == collectionViewModel.Id);

            collectionDb.ImageUrl = collectionViewModel.ImageUrl;
            collectionDb.CategoryId = categoryDb.Id;
            collectionDb.Name = collectionViewModel.Name;
            collectionDb.Description = collectionViewModel.Description;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCollection(string userName, long id)
        {
            var collection = await _context.Collections
                .Where(x => x.User.UserName == userName && x.Id == id)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Tags)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Comments)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Likes)
                .FirstAsync(x => x.Id == id);

            if (collection.Items.Count != 0)
            {
                foreach (var item in collection.Items)
                {
                    _context.Comments.RemoveRange(item.Comments);
                    _context.Likes.RemoveRange(item.Likes);
                }
                _context.Tags.RemoveRange(collection.Items.SelectMany(item => item.Tags));
                _context.RemoveRange(collection.Items);

                await _algolia.DeleteItems(collection.Items.Select(x => x.Id.ToString()));
            }

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

        private async Task<List<string>> GetAllCategories() => await _context.Categories.AsNoTracking().Select(x => x.Name).ToListAsync();
    }
}
