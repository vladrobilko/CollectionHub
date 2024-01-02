using CollectionHub.Models;

namespace CollectionHub.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> RegisterAsync(RegisterUserViewModel registerModel);

        Task<bool> LoginAsync(LoginUserViewModel loginModel);

        Task LogoutAsync();
    }
}
