using CollectionHub.DataManagement;
using CollectionHub.Helpers;
using CollectionHub.Models.ViewModels;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CollectionHub.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;

        private readonly SignInManager<User> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<bool> RegisterAsync(RegisterUserViewModel registerModel)
        {
            var result = await CreateAndSignInUserAsync(registerModel, CreateUser(registerModel));
            return result.Succeeded;
        }

        public async Task<bool> LoginAsync(LoginUserViewModel loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user == null)
            {
                loginModel.SetErrorMessage(null, user);
                return false;
            }
            var signInResult = await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);
            if (!user.IsBlocked && signInResult.Succeeded) await UpdateUserAsync(user);
            else loginModel.SetErrorMessage(signInResult, user);
            return user?.IsBlocked == false && signInResult.Succeeded;
        }

        public async Task LogoutAsync() => await _signInManager.SignOutAsync();

        private async Task<IdentityResult> CreateAndSignInUserAsync(RegisterUserViewModel registerModel, User user)
        {
            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            else
                registerModel.ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
            return result;
        }

        private async Task UpdateUserAsync(User user)
        {
            user.LastLoginDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }

        private User CreateUser(RegisterUserViewModel registerModel) =>
            new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = registerModel.Email,
                ViewName = registerModel.ViewName,
                Email = registerModel.Email,
                RegistrationDate = DateTime.UtcNow,
                LastLoginDate = DateTime.UtcNow,
                IsBlocked = false,
                IsAdmin = false
            };
    }
}
