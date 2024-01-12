using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectionHub.Controllers
{
    public class ItemController : Controller
    {
        private readonly ICollectionService _collectionService;

        public ItemController(ICollectionService collectionService)
        {
            _collectionService = collectionService;
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
            var formDataDictionary = new Dictionary<string, string>();

            foreach (var key in formCollection.Keys)
                formDataDictionary[key] = formCollection[key];
            return View();
        }
    }
}
