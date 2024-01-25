using CollectionHub.DataManagement;
using CollectionHub.Domain;
using CollectionHub.Models.ViewModels;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CollectionHub.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<UserDb> _userManager;

        private readonly SignInManager<UserDb> _signInManager;

        public AccountService(UserManager<UserDb> userManager, SignInManager<UserDb> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<bool> Register(RegisterUserViewModel registerModel)
        {
            var result = await CreateAndSignInUserAsync(registerModel, CreateUser(registerModel));
            return result.Succeeded;
        }

        public async Task<bool> Login(LoginUserViewModel loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);

            if (user == null)
            {
                loginModel.SetErrorMessage(null, user);
                return false;
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);

            if (!user.IsBlocked && signInResult.Succeeded)
            {
                await UpdateUserAsync(user);
            }
            else
            {
                await Logout();
                loginModel.SetErrorMessage(signInResult, user);
            }

            return user?.IsBlocked == false && signInResult.Succeeded;
        }

        public Task<AuthenticationProperties> GetGoogleExternalAuthProperties(string redirectUrl)
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

            return Task.FromResult(properties);
        }

        public async Task<bool> ExternalSignIn()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync() ?? throw new NotImplementedException();

            if (await TrySignInWithExternalInfoAsync(info))
            {
                return true;
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? throw new NotImplementedException();

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return await CreateAndSignInExternalUserAsync(info, email);
            }

            await _userManager.AddLoginAsync(user, info);

            await _signInManager.SignInAsync(user, isPersistent: false);

            return true;
        }

        public async Task Logout() => await _signInManager.SignOutAsync();

        private async Task<IdentityResult> CreateAndSignInUserAsync(RegisterUserViewModel registerModel, UserDb user)
        {
            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            else
            {
                registerModel.ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return result;
        }

        private async Task<bool> CreateAndSignInExternalUserAsync(ExternalLoginInfo info, string email)
        {
            var user = new UserDb { UserName = email, IsAdmin = false, IsBlocked = false, ViewName = email.Split('@')[0], Email = email };

            var createUserResult = await _userManager.CreateAsync(user);

            if (createUserResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return createUserResult.Succeeded;
        }

        private async Task<bool> TrySignInWithExternalInfoAsync(ExternalLoginInfo info)
        {
            var resultSignIn = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            return resultSignIn.Succeeded;
        }

        private async Task UpdateUserAsync(UserDb user)
        {
            user.LastLoginDate = DateTimeOffset.Now;

            await _userManager.UpdateAsync(user);
        }

        private UserDb CreateUser(RegisterUserViewModel registerModel) =>
            new UserDb
            {
                Id = Guid.NewGuid().ToString(),
                UserName = registerModel.Email,
                ViewName = registerModel.ViewName,
                Email = registerModel.Email,
                RegistrationDate = DateTimeOffset.Now,
                LastLoginDate = DateTimeOffset.Now,
                IsBlocked = false,
                IsAdmin = false
            };
    }
}
