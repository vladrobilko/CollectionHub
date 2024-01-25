using Microsoft.AspNetCore.Mvc;

namespace CollectionHub.Domain
{
    public static class ControllerExtensions
    {
        public static string GetUserNameFromContext(this Controller controller)
        {
            if (controller.User.Identity == null) throw new AccessViolationException();

            var isAdmin = controller.User.IsInRole("Admin");

            var userNameCookies = controller.Request.Cookies["UserName"];

            if (isAdmin && userNameCookies != null)
            {
                return userNameCookies;
            }

            return controller.User.Identity.Name ?? throw new NullReferenceException();
        }
    }
}
