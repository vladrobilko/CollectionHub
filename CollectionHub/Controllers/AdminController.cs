using CollectionHub.Helpers;
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
            if (await _adminService.IsUserBlocked(HttpContext.User.Identity.Name) || !HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(await _adminService.GetSortUsersAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> HandleAdminAction(string action, List<string> selectedUserEmails)
        {
            if (await _adminService.IsUserBlocked(HttpContext.User.Identity.Name))
            {
                return RedirectToAction("Login", "Account");
            }
            await _adminService.HandleAdminActionsAsync(action.ToUserManageActions(), selectedUserEmails);
            return RedirectToAction("Admin");
        }
    }
}
