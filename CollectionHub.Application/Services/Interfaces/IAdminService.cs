using CollectionHub.DataManagement;
using CollectionHub.Models;

namespace CollectionHub.Services.Interfaces
{
    public interface IAdminService
    {
        Task<List<UserDb>> GetSortUsersAsync();

        Task HandleAdminActionAsync(UserManageActions action, List<string> emails);

        Task<bool> IsUserBlockedOrNotAdmin(string? email);
    }
}
