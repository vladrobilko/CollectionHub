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

        public async Task<List<UserDb>> GetSortUsers()
        {
            var users = await _userManager.Users.AsNoTracking().ToListAsync();
            return users.OrderByDescending(u => u.IsAdmin).ToList();
        }

        public async Task HandleAdminAction(UserManageActions action, List<string> emails)//refactor
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

        public async Task<bool> IsUserBlockedOrNotAdmin(string? email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            return user == null || user.IsBlocked || !isAdmin;
        }

        private async Task ProcessAdminUsers(List<string> emails, bool isAdmin)
        {
            foreach (var email in emails)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    user.IsAdmin = isAdmin;
                    if (isAdmin) 
                    {
                        await _userManager.RemoveFromRoleAsync(user, "User");
                        await _userManager.AddToRoleAsync(user, "Admin"); 
                    }
                    else
                    {
                        await _userManager.RemoveFromRoleAsync(user, "Admin");
                        await _userManager.AddToRoleAsync(user, "User");
                    }
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
