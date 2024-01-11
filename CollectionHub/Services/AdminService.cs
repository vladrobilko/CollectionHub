using CollectionHub.DataManagement;
using CollectionHub.Models;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CollectionHub.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<UserDb> _userManager;

        public AdminService(UserManager<UserDb> userManager) => _userManager = userManager;

        public async Task<List<UserDb>> GetSortUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return users.OrderByDescending(u => u.IsAdmin).ToList();
        }

        public async Task HandleAdminActionsAsync(UserManageActions action, List<string> emails)//refactor
        {
            if (action == UserManageActions.Delete)
                await DeleteUsers(emails);
            if (action == UserManageActions.Block)
                await ProcessBlockingUsersAsync(emails, true);
            if (action == UserManageActions.Unblock)
                await ProcessBlockingUsersAsync(emails, false);
            if (action == UserManageActions.MakeAdmin)
                await ProcessAdminUsers(emails, true);
            if (action == UserManageActions.MakeUser)
                await ProcessAdminUsers(emails, false);
        }

        public async Task<bool> IsUserBlocked(string? email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user == null || user.IsBlocked;
        }

        private async Task ProcessAdminUsers(List<string> emails, bool isAdmin)
        {
            foreach (var email in emails)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    if (isAdmin) await _userManager.AddToRoleAsync(user, "Admin");
                    user.IsAdmin = isAdmin;
                    await _userManager.UpdateAsync(user);
                }
            }
        }

        private async Task ProcessBlockingUsersAsync(List<string> emails, bool isBlocked)
        {
            foreach (var email in emails)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    user.IsBlocked = isBlocked;
                    await _userManager.UpdateAsync(user);
                }
            }
        }

        private async Task DeleteUsers(List<string> emails)
        {
            foreach (var email in emails)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                    await _userManager.DeleteAsync(user);
            }
        }
    }
}
