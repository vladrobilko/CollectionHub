using CollectionHub.Models.Enums;
using CollectionHub.Models.ViewModels;

namespace CollectionHub.Services.Interfaces
{
    public interface ICollectionService
    {
        Task CreateCollection(CollectionViewModel collection, string userMail);

        Task<bool> CreateCollectionItemField(string userName, long id, DataType type, string name);

        Task<List<CollectionViewModel>> GetUserCollections(string userName);

        Task<CollectionViewModel> GetUserCollection(string userName, long id);

        Task<CollectionViewModel> GetEmptyCollectionViewModel();

        Task<List<CollectionViewModel>> GetLargestCollections();

        Task<CollectionViewModel> GetCollectionForRead(long id);

        Task EditCollection(string userName, CollectionViewModel collectionViewModel);

        Task DeleteCollection(long id);
    }
}
