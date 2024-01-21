using CollectionHub.DataManagement;
using CollectionHub.Models.ViewModels;

namespace CollectionHub.Domain.Interfaces
{
    public interface IAlgoliaIntegration
    {
        Task<List<ItemViewModel>> SearchItems(string query);

        Task CreateItem(ItemDb item);

        Task UpdateItem(ItemDb item);

        Task DeleteItem(long id);

        Task DeleteItems(IEnumerable<string> ids);
    }
}
