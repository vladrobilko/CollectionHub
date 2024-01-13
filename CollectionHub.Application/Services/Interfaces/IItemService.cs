using Microsoft.AspNetCore.Http;

namespace CollectionHub.Services.Interfaces
{
    public interface IItemService
    {
        Task CreateItem(string userName, IFormCollection formCollection);
    }
}
