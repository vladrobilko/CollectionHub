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
            await _itemService.CreateItem(HttpContext.User.Identity.Name, formCollection);
            return View();
        }
    }
}
