﻿using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectionHub.Controllers
{
    public class ItemController : Controller
    {
        private readonly ICollectionService _collectionService;

        private readonly IItemService _itemService;

        public ItemController(ICollectionService collectionService, IItemService itemService)
        {
            _collectionService = collectionService;
            _itemService = itemService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateItem(long collectionId)
        {
            var collection = await _collectionService.GetUserCollection(HttpContext.User.Identity.Name, collectionId);

            return View(collection);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateItem(IFormCollection formCollection)
        {
            var collectionId = await _itemService.CreateItem(HttpContext.User.Identity.Name, formCollection);

            return RedirectToAction("GetCollection", "Collection", new { id = collectionId });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> EditItem(int collectionId, int selectedItemId)
        {
            var item = await _itemService.GetItem(selectedItemId, collectionId, HttpContext.User.Identity.Name);

            return View(item);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditItem(IFormCollection formCollection)
        {
            var collectionId = await _itemService.EditItem(HttpContext.User.Identity.Name, formCollection);

            return RedirectToAction("GetCollection", "Collection", new { id = collectionId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteItem(int collectionId, int selectedItemId)
        {
            await _itemService.DeleteItem(selectedItemId);

            return RedirectToAction("GetCollection", "Collection", new { id = collectionId });
        }
    }
}
