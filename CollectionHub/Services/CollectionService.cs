using CollectionHub.DataManagement;
using CollectionHub.Helpers;
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
            return collection.ToCollectionViewModel();
        }

        public async Task CreateCollection(CollectionViewModel collection, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var category = await _context.Categories.FirstAsync(x => x.Name == collection.Category);
            await _context.Collections.AddAsync(CreateCollectionDbInstance(user, collection, category));
            await _context.SaveChangesAsync();
        }

        private CollectionDb CreateCollectionDbInstance(UserDb user, CollectionViewModel collection,CategoryDb category)
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
    }
}
