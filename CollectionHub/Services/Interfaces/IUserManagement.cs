using CollectionHub.Models;

namespace CollectionHub.Services.Interfaces
{
    public interface IUserManagement
    {
        Task<List<User>> GetSortUsersAsync();

        Task HandleUserManageActionsAsync(UserManageActions action, List<string> emails);

        Task<bool> IsUserBlocked(string? email);
    }
}
