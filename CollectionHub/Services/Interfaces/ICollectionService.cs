using CollectionHub.Models.ViewModels;

namespace CollectionHub.Services.Interfaces
{
    public interface ICollectionService
    {
        Task<List<string>> GetAllCategories();

        Task CreateCollection(CollectionViewModel collection, string userMail);

        Task<List<CollectionViewModel>> GetUserCollections(string userName);

        Task<CollectionViewModel> GetUserCollection(string userName, long id);
    }
}
