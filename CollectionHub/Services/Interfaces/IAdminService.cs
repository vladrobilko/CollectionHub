using CollectionHub.DataManagement;
using CollectionHub.Models;

namespace CollectionHub.Services.Interfaces
{
    public interface IAdminService
    {
        Task<List<UserDb>> GetSortUsersAsync();

        Task HandleAdminActionsAsync(UserManageActions action, List<string> emails);

        Task<bool> IsUserBlocked(string? email);
    }
}
