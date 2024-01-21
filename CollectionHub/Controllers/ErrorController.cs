using Microsoft.AspNetCore.Mvc;

namespace CollectionHub.Controllers
{
    public class ErrorController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [IgnoreAntiforgeryToken]
        public IActionResult Error() => View();
    }
}
