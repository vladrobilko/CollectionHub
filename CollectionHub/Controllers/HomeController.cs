﻿using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace CollectionHub.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MyCollections", "Collection");
            }
            return View();
        }

        [HttpGet]
        public IActionResult ChangeLanguage(string? culture)
        {
            if (culture != null)
            {
                SetCookiesForLanguage(culture);
            }
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }

        [HttpGet]
        public IActionResult ChangeTheme(string? theme)
        {
            if (theme != null)
            {
                SetCookiesForTheme(theme);
            }
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }

        private void SetCookiesForTheme(string? theme)//throw to business logic?
        {
            Response.Cookies.Append(
                "ThemePreference",
                theme,
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
        }

        private void SetCookiesForLanguage(string? culture)//throw to business logic?
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
        }
    }
}
