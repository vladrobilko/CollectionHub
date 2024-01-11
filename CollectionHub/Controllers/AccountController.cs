using CollectionHub.Models.ViewModels;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CollectionHub.Controllers
{

    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService) => _accountService = accountService;

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MyCollections", "Collection");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserViewModel registerModel)
        {
            if (ModelState.IsValid && await _accountService.RegisterAsync(registerModel))
            {
                return RedirectToAction("MyCollections", "Collection");
            }
            return View("Register", registerModel);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MyCollections", "Collection");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserViewModel loginModel)
        {
            if (ModelState.IsValid && await _accountService.LoginAsync(loginModel))
            {
                return RedirectToAction("MyCollections", "Collection");
            }
            return View("Login", loginModel);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}