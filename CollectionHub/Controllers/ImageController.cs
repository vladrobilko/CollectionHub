using CollectionHub.Models.ViewModels;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectionHub.Controllers
{
    public class ImageController : Controller
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService) => _imageService = imageService;

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UploadImage(CollectionViewModel collectionViewModel)
        {
            var imagePath = _imageService.UploadImageToAzureAndGiveImageLink(collectionViewModel.File);
            TempData["ImageUrl"] = imagePath;
            return RedirectToAction("CreateCollection", "Collection");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateImage(CollectionViewModel collectionViewModel, long collectionId)
        {
            var imagePath = _imageService.UploadImageToAzureAndGiveImageLink(collectionViewModel.File);
            TempData["ImageUrl"] = imagePath;
            return RedirectToAction("EditCollection", "Collection", new { id = collectionId });
        }
    }
}
