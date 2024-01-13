using Microsoft.AspNetCore.Http;

namespace CollectionHub.Services.Interfaces
{
    public interface IItemService
    {
        Task<long> CreateItem(string userName, IFormCollection formCollection);

        Task<List<List<string>>> GetCollectionItems(long collectionId, Dictionary<string, string> fieldNames);

        Task DeleteItem(long id);
    }
}
