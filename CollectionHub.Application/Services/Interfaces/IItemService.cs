using CollectionHub.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace CollectionHub.Services.Interfaces
{
    public interface IItemService
    {
        Task<long> CreateItem(string userName, IFormCollection formCollection);

        Task<List<List<string>>> GetCollectionItems(long collectionId, Dictionary<string, string> fieldNames);
        
        Task<List<ItemViewModel>> GetRecentlyAddedItemsForRead();

        Task<ItemViewModel> GetItem(long itemId, long collectionId);

        Task<long> EditItem(string userName, IFormCollection formCollection);

        Task ProcessLikeItem(string userName, long itemId);

        Task AddComment(string userName, long itemId, string text);

        Task<List<ItemViewModel>> SearchItems(string query);

        Task DeleteItem(string userName, long id);
    }
}
