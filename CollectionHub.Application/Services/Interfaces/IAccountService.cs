using CollectionHub.Models.ViewModels;

namespace CollectionHub.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> Register(RegisterUserViewModel registerModel);

        Task<bool> Login(LoginUserViewModel loginModel);

        Task Logout();
    }
}
