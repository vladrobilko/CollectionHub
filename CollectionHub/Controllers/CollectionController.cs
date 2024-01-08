using CollectionHub.Helpers;
using CollectionHub.Models.ViewModels;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectionHub.Controllers
{
    public class CollectionController : Controller
    {
        private readonly IImageService _imageService;

        private readonly ICollectionService _collectionService;

        public CollectionController(IImageService imageService, ICollectionService collectionService)
        {
            _imageService = imageService;
            _collectionService = collectionService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyCollections()
        {
            var collections = await _collectionService.GetUserCollections(HttpContext.User.Identity.Name);
            return View(collections);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateCollection()
        {
            var categories = await _collectionService.GetAllCategories();
            var viewModel = new CollectionViewModel { Categories = categories.ToSelectListItem() };
            if (TempData.TryGetValue("ImageUrl", out var imagePath)) viewModel.ImageUrl = (string)imagePath;
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCollection(CollectionViewModel collectionViewModel)
        {
            if (ModelState.IsValid)
            {
                await _collectionService.CreateCollection(collectionViewModel, HttpContext.User.Identity.Name);
                return RedirectToAction("MyCollections");
            }
            var categories = await _collectionService.GetAllCategories();
            collectionViewModel.Categories = categories.ToSelectListItem();
            return View(collectionViewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCollection(int id)
        {
            var collection = await _collectionService.GetUserCollection(HttpContext.User.Identity.Name, id);
            return View(collection);
        }

        [HttpPost]
        public IActionResult AddItemField(string collectionId, string type, string name)
        {
            if (string.IsNullOrEmpty(type))
            {
                ModelState.AddModelError("type", "Please select a type.");
            }

            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("name", "Please enter a type name.");
            }

            var collection = _collectionService.GetUserCollection(HttpContext.User.Identity.Name, long.Parse(collectionId));

            if (ModelState.IsValid)
            {
                //save to db
                return View("GetCollection", collection);
            }

            // If there are validation errors, redisplay the form with error messages
            return View("GetCollection", collection);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(CollectionViewModel collectionViewModel)
        {
            var imagePath = await _imageService.UploadImageToAzureAndGiveImageLink(collectionViewModel.File);
            TempData["ImageUrl"] = imagePath;
            return RedirectToAction("CreateCollection");
        }
    }
}
