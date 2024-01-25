using CollectionHub.Domain;
using CollectionHub.Models;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CollectionHub.Services
{
    public class ImageService : IImageService
    {
        private readonly AzureOptions _azureOptions;

        public ImageService(IOptions<AzureOptions> azureOptions) => _azureOptions = azureOptions.Value;

        public string UploadImageToAzureAndGiveImageLink(IFormFile? file) => AzureImageUploader.Upload(file, _azureOptions);
    }
}
