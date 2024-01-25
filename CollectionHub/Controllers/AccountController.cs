using CollectionHub.Models.ViewModels;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
            if (ModelState.IsValid && await _accountService.Register(registerModel))
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
            if (ModelState.IsValid && await _accountService.Login(loginModel))
            {
                return RedirectToAction("MyCollections", "Collection");
            }

            return View("Login", loginModel);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _accountService.Logout();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleSignIn()
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");

            var properties = await _accountService.GetGoogleExternalAuthProperties(redirectUrl);

            return Challenge(properties, "Google");
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? remoteError)
        {
            if (remoteError != null)
            {
                return View("Login", new LoginUserViewModel { ErrorMessage = remoteError });
            }

            if (await _accountService.ExternalSignIn())
            {
                return RedirectToAction("MyCollections", "Collection");
            }

            return RedirectToAction("LogIn", "User");
        }
    }
}