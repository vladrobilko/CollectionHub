using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectionHub.Controllers
{
    public class CollectionController : Controller
    {

        [Authorize]
        [HttpGet]
        public IActionResult MyCollections()
        {
            return View();
        }


        [Authorize]
        [HttpGet]
        public IActionResult CreateCollection(string? userName)
        {
            return View();
        }
    }
}
