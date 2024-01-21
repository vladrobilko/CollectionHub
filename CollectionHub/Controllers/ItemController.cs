using CollectionHub.Domain;
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
        public async Task<IActionResult> SearchItems(string query) => View(await _itemService.SearchItems(query));

        [HttpGet]
        public async Task<IActionResult> GetItem(long itemId, long collectionId) => View(await _itemService.GetItem(itemId, collectionId));

        [HttpGet]
        public async Task<IActionResult> RecentlyAdded() => View(await _itemService.GetRecentlyAddedItemsForRead());

        [HttpPost]
        public async Task<IActionResult> AddComment(string comment, long itemId, long collectionId)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            await _itemService.AddComment(this.GetUserNameFromContext(), itemId, comment);

            return RedirectToAction("GetItem", new { itemId, collectionId });
        }

        [HttpGet]
        public async Task<IActionResult> PressLike(long itemId, long collectionId)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            await _itemService.ProcessLikeItem(this.GetUserNameFromContext(), itemId);

            return RedirectToAction("GetItem", new { itemId, collectionId });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CreateItem(long collectionId) => View(await _collectionService.GetUserCollection(this.GetUserNameFromContext(), collectionId));

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateItem(IFormCollection formCollection)
        {
            var collectionId = await _itemService.CreateItem(this.GetUserNameFromContext(), formCollection);

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

            TempData["ErrorMessage"] = ErrorMessageManager.ItemSelection;

            return RedirectToAction("GetCollection", "Collection", new { id = collectionId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditItem(IFormCollection formCollection)
        {
            var collectionId = await _itemService.EditItem(this.GetUserNameFromContext(), formCollection);

            return RedirectToAction("GetCollection", "Collection", new { id = collectionId });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteItem(int collectionId, int selectedItemId)
        {
            if (ModelState.IsValid)
            {
                await _itemService.DeleteItem(this.GetUserNameFromContext(), selectedItemId);

                return RedirectToAction("GetCollection", "Collection", new { id = collectionId });
            }

            TempData["ErrorMessage"] = ErrorMessageManager.NoItemSelected;

            return RedirectToAction("GetCollection", "Collection", new { id = collectionId });
        }
    }
}
