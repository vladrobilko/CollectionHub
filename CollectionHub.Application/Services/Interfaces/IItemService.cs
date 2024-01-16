using CollectionHub.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace CollectionHub.Services.Interfaces
{
    public interface IItemService
    {
        Task<long> CreateItem(string userName, IFormCollection formCollection);

        Task<List<List<string>>> GetCollectionItems(long collectionId, Dictionary<string, string> fieldNames);

        Task DeleteItem(long id);

        Task<ItemViewModel> GetItem(long itemId, long collectionId);

        Task<long> EditItem(string userName, IFormCollection formCollection);

        Task<List<ItemViewModel>> GetRecentlyAddedItemsForRead();

        Task ProcessLikeItem(string userName, long itemId);
    }
}
