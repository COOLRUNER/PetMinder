using Microsoft.AspNetCore.SignalR;
using PetMinder.Shared.DTO;
using System.Threading.Tasks;

namespace PetMinder.Api.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessageToConversation(string conversationId, MessageDTO message)
        {
            await Clients.Group(conversationId).SendAsync("ReceiveMessage", message);
        }

        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }
    }
}
