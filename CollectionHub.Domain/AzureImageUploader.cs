using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CollectionHub.Models;
using FileTypeChecker.Extensions;
using Microsoft.AspNetCore.Http;

namespace CollectionHub.Domain
{
    public static class AzureImageUploader
    {
        public static string Upload(IFormFile? file, AzureOptions azureOptions)
        {
            EnsureFileIsValid(file);

            using var fileUploadStream = new MemoryStream();
            file.CopyTo(fileUploadStream);
            fileUploadStream.Position = 0;

            var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            UploadFileToAzure(fileUploadStream, uniqueName, azureOptions);

            return azureOptions.BlobURL + "/" + uniqueName;
        }

        private static void EnsureFileIsValid(IFormFile? file)
        {
            if (file == null || file.FileName == null)
            {
                throw new InvalidOperationException("Invalid file.");
            }

            if (!file.OpenReadStream().IsImage())
            {
                throw new FormatException("Invalid image format.");
            }
        }

        private static void UploadFileToAzure(Stream fileUploadStream, string uniqueName, AzureOptions azureOptions)
        {
            var blobContainerClient = new BlobContainerClient(azureOptions.ConnectionString, azureOptions.Container);
            var blobClient = blobContainerClient.GetBlobClient(uniqueName);
            blobClient.Upload(fileUploadStream, new BlobUploadOptions()
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/bitmap"
                }
            }, cancellationToken: default);
        }
    }
}
