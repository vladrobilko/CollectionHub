using Microsoft.AspNetCore.Http;

namespace CollectionHub.Services.Interfaces
{
    public interface IImageService
    {
        string UploadImageToAzureAndGiveImageLink(IFormFile file);
    }
}