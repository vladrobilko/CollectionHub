using Microsoft.AspNetCore.Http;

namespace CollectionHub.Services.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageToAzureAndGiveImageLink(IFormFile file);
    }
}