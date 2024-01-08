namespace CollectionHub.Services.Interfaces
{
    public interface ICollectionService
    {
        Task<List<string>> GetAllCategories();
    }
}
