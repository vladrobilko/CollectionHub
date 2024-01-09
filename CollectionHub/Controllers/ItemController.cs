using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectionHub.Controllers
{
    public class ItemController : Controller
    {
        [Authorize]
        [HttpGet]
        public IActionResult CreateItem(long collectionId)
        {
            return View();
        }
    }
}
