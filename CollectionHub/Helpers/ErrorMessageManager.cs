using CollectionHub.DataManagement;
using CollectionHub.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace CollectionHub.Helpers
{
    public static class ErrorMessageManager
    {
        public static void SetErrorMessage(this LoginUserViewModel loginUser, SignInResult? result, User? user)
        {
            if (result?.IsLockedOut == true)
                loginUser.ErrorMessage = "Your account is locked due to too many failed attempts. Please try again later.";
            else if (result?.IsNotAllowed == true)
                loginUser.ErrorMessage = "You are not allowed to sign in. Please contact support for assistance.";
            else if (!result?.Succeeded == true)
                loginUser.ErrorMessage = "Incorrect password. Please try again.";
            else if (user?.IsBlocked == true)
                loginUser.ErrorMessage = "Sorry, your account is currently blocked.";
            else if (user == null)
                loginUser.ErrorMessage = "Invalid email. Please try again.";
            else
                loginUser.ErrorMessage = "Invalid email or password. Please try again.";
        }
    }
}
