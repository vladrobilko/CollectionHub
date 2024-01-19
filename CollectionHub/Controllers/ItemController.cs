using CollectionHub.Services.Interfaces;
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

        [HttpGet]
        public async Task<IActionResult> SearchItems(string query)
        {
            return View(await _itemService.SearchItems(query));
        }

        [HttpGet]
        public async Task<IActionResult> GetItem(long itemId, long collectionId)
        {
            return View(await _itemService.GetItem(itemId, collectionId));
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string comment, long itemId, long collectionId)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            await _itemService.AddComment(HttpContext.User.Identity.Name, itemId, comment);

            return RedirectToAction("GetItem", new { itemId, collectionId });
        }

        [HttpGet]
        public async Task<IActionResult> PressLike(long itemId, long collectionId)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            await _itemService.ProcessLikeItem(HttpContext.User.Identity.Name, itemId);

            return RedirectToAction("GetItem", new { itemId, collectionId });
        }

        [HttpGet]
        public async Task<IActionResult> RecentlyAdded()
        {
            return View(await _itemService.GetRecentlyAddedItemsForRead());
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
            if (ModelState.IsValid)
            {
                return View(await _itemService.GetItem(selectedItemId, collectionId));
            }

            TempData["ErrorMessage"] = "Please select an item to edit";

            return RedirectToAction("GetCollection", "Collection", new { id = collectionId });
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
            if (ModelState.IsValid)
            {
                await _itemService.DeleteItem(selectedItemId);

                return RedirectToAction("GetCollection", "Collection", new { id = collectionId });
            }

            TempData["ErrorMessage"] = "Please select an item to delete";

            return RedirectToAction("GetCollection", "Collection", new { id = collectionId });
        }
    }
}
