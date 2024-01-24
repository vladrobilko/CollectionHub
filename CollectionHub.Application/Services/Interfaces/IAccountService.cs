using CollectionHub.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;

namespace CollectionHub.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> Register(RegisterUserViewModel registerModel);

        Task<bool> Login(LoginUserViewModel loginModel);

        Task<AuthenticationProperties> GetGoogleExternalAuthProperties(string redirectUrl);

        Task<bool> ExternalSignIn();

        Task Logout();
    }
}
