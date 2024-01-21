using CollectionHub.DataManagement;
using CollectionHub.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace CollectionHub.Domain
{
    public static class ErrorMessageManager
    {
        public const string FieldLimit = "You can only add up to 3 fields of the same type, and names must be unique.";

        public const string TypeSelection = "Please select a type and enter a type name.";

        public const string ItemSelection = "Please select an item to edit.";

        public static void SetErrorMessage(this LoginUserViewModel loginUser, SignInResult? result, UserDb? user) =>
            loginUser.ErrorMessage = result switch
            {
                { IsLockedOut: true } => "Your account is locked due to too many failed attempts. Please try again later.",
                { IsNotAllowed: true } => "You are not allowed to sign in. Please contact support for assistance.",
                { Succeeded: false } => "Incorrect password. Please try again.",
                _ when user?.IsBlocked == true => "Sorry, your account is currently blocked.",
                null => "Invalid email. Please try again.",
                _ => "Invalid email or password. Please try again.",
            };
    }
}
