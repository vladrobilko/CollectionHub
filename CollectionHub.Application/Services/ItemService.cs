using CollectionHub.DataManagement;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using CollectionHub.Domain.Converters;
using CollectionHub.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using CollectionHub.Domain.Interfaces;
using CollectionHub.Domain;
using Microsoft.AspNetCore.SignalR;

namespace CollectionHub.Services
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<UserDb> _userManager;

        private readonly IAlgoliaIntegration _algolia;

        private readonly IItemMapper _itemMapper;

        private readonly IHubContext<CommentsHub> _commentsHubContext;

        public ItemService(ApplicationDbContext context, UserManager<UserDb> userManager, IAlgoliaIntegration algolia, IItemMapper itemMapper, IHubContext<CommentsHub> commentsHubContext)
        {
            _context = context;
            _userManager = userManager;
            _algolia = algolia;
            _itemMapper = itemMapper;
            _commentsHubContext = commentsHubContext;
        }

        public async Task<long> CreateItem(string userName, IFormCollection formCollection)
        {
            var collectionId = formCollection.ToId();

            await CheckIsUserHasCollection(userName, collectionId);

            var newItem = new ItemDb();

            _itemMapper.MapFormFieldsToItem(newItem, formCollection, isItemNew: true);

            newItem.CollectionId = collectionId;
            newItem.CreationDate = DateTimeOffset.Now;

            await _context.AddAsync(newItem);
            await _context.SaveChangesAsync();

            await _algolia.CreateItem(newItem);

            return collectionId;
        }

        public async Task<List<List<string>>> GetCollectionItems(long collectionId, Dictionary<string, string> fieldNames)
        {
            var items = await _context.Items
                .AsNoTracking()
                .Where(item => item.CollectionId == collectionId)
                .Include(item => item.Tags)
                .ToListAsync();

            var listItems = _itemMapper.MapItemValuesToLists(items, fieldNames);

            return listItems;
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

        public async Task<ItemViewModel> GetItem(long itemId, long collectionId)
        {
            var collection = await _context.Collections
                .AsNoTracking()
                .Where(x => x.Id == collectionId)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Tags)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Likes)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Comments)
                    .ThenInclude(x => x.User)
                .FirstAsync(x => x.Id == collectionId);

            var item = collection.Items.First(x => x.Id == itemId);

            var allHeadersWihValues = _itemMapper.MapCollectionToItemProperties(collection, item);

            return item.ToItemViewModel(itemId, collectionId, allHeadersWihValues);
        }

        public async Task<long> EditItem(string userName, IFormCollection formCollection)
        {
            var itemId = formCollection.ToId();

            var itemToUpdate = await _context.Items
                .Include(x => x.Tags)
                .FirstAsync(x => x.Id == itemId);

            _context.Tags.RemoveRange(itemToUpdate.Tags);

            _itemMapper.MapFormFieldsToItem(itemToUpdate, formCollection, isItemNew: false);

            await _context.SaveChangesAsync();

            await _algolia.UpdateItem(itemToUpdate);

            return itemToUpdate.CollectionId;
        }

        public async Task ProcessLikeItem(string userName, long itemId)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var existingLike = await _context.Likes
                .Where(like => like.UserId == user.Id && like.ItemId == itemId)
                .FirstOrDefaultAsync();

            if (existingLike == null)
            {
                _context.Likes.Add(CreateLikeDbInstance(user, itemId));
            }
            else
            {
                _context.Likes.Remove(existingLike);
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddComment(string userName, long itemId, string text)
        {
            var user = await _userManager.FindByNameAsync(userName);

            var commentDb = CreateCommentDbInstance(user, itemId, text);

            _context.Add(commentDb);

            await _context.SaveChangesAsync();

            await _commentsHubContext.Clients.Group($"Item_{itemId}").SendAsync("Receive", commentDb.ToCommentViewModel(user));
        }

        public async Task<List<ItemViewModel>> SearchItems(string query) => await _algolia.SearchItems(query);

        public async Task DeleteItem(string userName, long id)
        {
            var item = await _context.Items
                .Where(x => x.Collection.User.UserName == userName && x.Id == id)
                .Include(x => x.Tags)
                .Include(x => x.Likes)
                .Include(x => x.Comments)
                .FirstOrDefaultAsync(x => x.Id == id);

            _context.Likes.RemoveRange(item.Likes);
            _context.Comments.RemoveRange(item.Comments);
            _context.Tags.RemoveRange(item.Tags);
            _context.Items.Remove(item);

            await _context.SaveChangesAsync();
            await _algolia.DeleteItem(id);
        }

        private async Task CheckIsUserHasCollection(string userName, long collectionId)
        {
            var collection = await _context.Collections
                .Where(x => x.User.UserName == userName)
                .FirstOrDefaultAsync(x => x.Id == collectionId) ?? throw new AccessViolationException();
        }

        private LikeDb CreateLikeDbInstance(UserDb user, long itemId) =>
            new LikeDb
            {
                UserId = user.Id,
                ItemId = itemId
            };

        private CommentDb CreateCommentDbInstance(UserDb user, long itemId, string text) =>
            new CommentDb
            {
                UserId = user.Id,
                ItemId = itemId,
                Text = text,
                CreationDate = DateTimeOffset.Now
            };
    }
}
