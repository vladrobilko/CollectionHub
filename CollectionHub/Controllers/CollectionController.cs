﻿using CollectionHub.Domain;
using CollectionHub.Domain.Converters;
using CollectionHub.Models.ViewModels;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectionHub.Controllers
{
    public class CollectionController : Controller
    {
        private readonly ICollectionService _collectionService;

        public CollectionController(ICollectionService collectionService) => _collectionService = collectionService;

        [HttpGet]
        public async Task<IActionResult> LargestCollections() => View(await _collectionService.GetLargestCollections());

        [HttpGet]
        public async Task<IActionResult> GetCollectionForRead(int id) => View(await _collectionService.GetCollectionForRead(id));

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyCollections() => View(await _collectionService.GetUserCollections(this.GetUserNameFromContext()));

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateCollection()
        {
            var viewModel = await _collectionService.GetEmptyCollectionViewModel();

            if (TempData.TryGetValue("ImageUrl", out var imagePath) && imagePath != null)
            {
                viewModel.ImageUrl = (string)imagePath;
            }

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCollection(CollectionViewModel collectionViewModel)
        {
            if (ModelState.IsValid)
            {
                await _collectionService.CreateCollection(collectionViewModel, this.GetUserNameFromContext());

                return RedirectToAction("MyCollections");
            }

            return View(await _collectionService.GetEmptyCollectionViewModel());
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCollection(int id)
        {
            var errorMessage = TempData["ErrorMessage"] as string;

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ModelState.AddModelError("error", errorMessage);
            }

            return View(await _collectionService.GetUserCollection(this.GetUserNameFromContext(), id));
        }

        [Authorize]
        public async Task<IActionResult> ExportCSV(long collectionId)
        {
            var collection = await _collectionService.GetUserCollection(this.GetUserNameFromContext(), collectionId);

            return File(collection.ToCsvBytes(), "text/csv", $"{collection.Name}.csv");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCollectionItemField(long collectionId, string type, string name)
        {
            if (ModelState.IsValid)
            {
                var result = await _collectionService.CreateCollectionItemField(this.GetUserNameFromContext(), collectionId, type.ToDataType(), name);
                if (!result)
                {
                    TempData["ErrorMessage"] = ErrorMessageManager.FieldLimit;
                }
                return RedirectToAction("GetCollection", new { id = collectionId });
            }

            TempData["ErrorMessage"] = ErrorMessageManager.TypeSelection;

            return RedirectToAction("GetCollection", new { id = collectionId });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteCollection(long id)
        {
            await _collectionService.DeleteCollection(this.GetUserNameFromContext(), id);

            return RedirectToAction("MyCollections");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditCollection(long id)
        {
            var collection = await _collectionService.GetUserCollection(this.GetUserNameFromContext(), id);

            if (TempData.TryGetValue("ImageUrl", out var imagePath) && imagePath != null)
            {
                collection.ImageUrl = (string)imagePath;
            }

            return View(collection);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditCollection(CollectionViewModel collectionViewModel)
        {
            if (ModelState.IsValid)
            {
                await _collectionService.EditCollection(this.GetUserNameFromContext(), collectionViewModel);

                return RedirectToAction("MyCollections");
            }

            return View(await _collectionService.GetEmptyCollectionViewModel());
        }
    }
}
