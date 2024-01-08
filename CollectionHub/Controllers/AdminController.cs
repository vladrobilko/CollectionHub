using CollectionHub.Helpers;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectionHub.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserManagementService _userManagement;

        public AdminController(IUserManagementService userManagement) => _userManagement = userManagement;

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Admin()
        {
            if (await _userManagement.IsUserBlocked(HttpContext.User.Identity.Name) || !HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");
            return View(await _userManagement.GetSortUsersAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> HandleAdminAction(string action, List<string> selectedUserEmails)
        {
            if (await _userManagement.IsUserBlocked(HttpContext.User.Identity.Name))
                return RedirectToAction("Login", "Account");
            await _userManagement.HandleUserManageActionsAsync(action.ToUserManageActions(), selectedUserEmails);//makeadmin
            return RedirectToAction("Admin");
        }
    }
}
