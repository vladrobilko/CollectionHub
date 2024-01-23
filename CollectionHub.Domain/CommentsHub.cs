using Microsoft.AspNetCore.SignalR;

namespace CollectionHub.Domain
{
    public class CommentsHub : Hub
    {
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
