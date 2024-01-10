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
            var errorMessage = TempData["ErrorMessage"] as string;
            if (!string.IsNullOrEmpty(errorMessage))            
                ModelState.AddModelError("error", errorMessage);            
            var collection = await _collectionService.GetUserCollection(HttpContext.User.Identity.Name, id);
            return View(collection);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCollectionItemField(long collectionId, string type, string name)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(name))
                TempData["ErrorMessage"] = "Please select a type and enter a type name.";            
            if (ModelState.IsValid)
            {
                var result = await _collectionService.AddCollectionItemField(HttpContext.User.Identity.Name, collectionId, type.ToDataType(), name);
                if (!result)                
                    TempData["ErrorMessage"] = "You can only add up to 3 fields of the same type, and names must be unique.";                
                return RedirectToAction("GetCollection", new { id = collectionId });
            }
            return RedirectToAction("GetCollection", new { id = collectionId });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteCollection(long id)
        {
            await _collectionService.DeleteCollection(id);
            return RedirectToAction("MyCollections");
        }
    }
}
