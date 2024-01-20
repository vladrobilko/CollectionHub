using CollectionHub.DataManagement;
using CollectionHub.Models;

namespace CollectionHub.Services.Interfaces
{
    public interface IAdminService
    {
        Task<List<UserDb>> GetSortUsers();

        Task HandleAdminAction(UserManageActions action, List<string> emails);

        Task<bool> IsUserBlockedOrNotAdmin(string? email);
    }
}
