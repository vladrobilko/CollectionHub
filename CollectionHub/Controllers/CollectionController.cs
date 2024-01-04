using CollectionHub.Models;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CollectionHub.Controllers
{
    public class CollectionController : Controller
    {
        private readonly IImageService _imageService;

        public CollectionController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult MyCollections()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult CreateCollection()
        {
            var viewModel = new CollectionViewModel();
            viewModel.Themes = GetThemes();
            if (TempData.TryGetValue("ImagePath", out var imagePath)) viewModel.ImageLink = (string)imagePath;            
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateCollection(CollectionViewModel collectionViewModel)
        {
            if (ModelState.IsValid)
            {
                //save to db
                throw new NotImplementedException();
            }
            collectionViewModel.Themes = GetThemes(); //get themes from db 
            return View(collectionViewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UploadImage(CollectionViewModel collectionViewModel)
        {
            var imagePath = _imageService.UploadImageToAzureAndGiveImageLink(collectionViewModel.File);
            TempData["ImagePath"] = imagePath;
            return RedirectToAction("CreateCollection");
        }

        private List<SelectListItem> GetThemes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Books"
                },
                new SelectListItem
                {
                    Text = "Movies"
                },
                new SelectListItem
                {
                    Text = "Coins"
                },
                new SelectListItem
                {
                    Text = "Watches"
                },
                new SelectListItem
                {
                    Text = "Board games"
                },
                new SelectListItem
                {
                    Text = "Model cars"
                },
                new SelectListItem
                {
                    Text = "Model airplanes"
                }
            };
        }
    }
}
