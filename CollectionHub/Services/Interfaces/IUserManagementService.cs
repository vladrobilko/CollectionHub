using CollectionHub.DataManagement;
using CollectionHub.Models;

namespace CollectionHub.Services.Interfaces
{
    public interface IUserManagementService
    {
        Task<List<User>> GetSortUsersAsync();

        Task HandleUserManageActionsAsync(UserManageActions action, List<string> emails);

        Task<bool> IsUserBlocked(string? email);
    }
}
