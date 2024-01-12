using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CollectionHub.Models;
using CollectionHub.Services.Interfaces;
using FileTypeChecker.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CollectionHub.Services
{
    public class ImageService : IImageService
    {
        private readonly AzureOptions _azureOptions;

        public ImageService(IOptions<AzureOptions> azureOptions) => _azureOptions = azureOptions.Value;

        public async Task<string> UploadImageToAzureAndGiveImageLink(IFormFile file)
        {
            EnsureFileIsValid(file);

            using var fileUploadStream = new MemoryStream();
            await file.CopyToAsync(fileUploadStream);
            fileUploadStream.Position = 0;

            var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            await UploadFileToAzure(fileUploadStream, uniqueName);

            return _azureOptions.BlobURL + "/" + uniqueName;
        }

        private async Task UploadFileToAzure(Stream fileUploadStream, string uniqueName)
        {
            var blobContainerClient = new BlobContainerClient(_azureOptions.ConnectionString, _azureOptions.Container);
            var blobClient = blobContainerClient.GetBlobClient(uniqueName);
            await blobClient.UploadAsync(fileUploadStream, new BlobUploadOptions()
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/bitmap"
                }
            }, cancellationToken: default);
        }

        private void EnsureFileIsValid(IFormFile file)
        {
            if (file == null || file.FileName == null)
            {
                throw new NotImplementedException();
            }
            if (!file.OpenReadStream().IsImage())
            {
                throw new FormatException();
            }
        }
    }
}
