using CollectionHub.Domain.Converters;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectionHub.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService) => _adminService = adminService;

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Admin()
        {
            if (await _adminService.IsUserBlockedOrNotAdmin(HttpContext.User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }

            return View(await _adminService.GetSortUsers());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> HandleAdminAction(string action, List<string> selectedUserEmails)
        {
            if (await _adminService.IsUserBlockedOrNotAdmin(HttpContext.User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }

            await _adminService.HandleAdminAction(action.ToUserManageActions(), selectedUserEmails);

            return RedirectToAction("Admin");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult ControlUser(string userName)
        {
            Response.Cookies.Append(
                "UserName",
                userName,
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return RedirectToAction("MyCollections", "Collection");
        }
    }
}
