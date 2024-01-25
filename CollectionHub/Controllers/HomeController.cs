using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace CollectionHub.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
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

        private void SetCookiesForTheme(string theme)
        {
            Response.Cookies.Append(
                "ThemePreference",
                theme,
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
        }

        private void SetCookiesForLanguage(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
        }
    }
}
