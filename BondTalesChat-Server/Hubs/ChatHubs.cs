using BondTalesChat_Server.models;
using BondTalesChat_Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace BondTalesChat_Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;

        public ChatHub(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<Message> SendMessage(int senderId, int groupId, int receiverId, string messageText)
        {
            return await _messageService.SaveAndBroadcastAsync(senderId, groupId, receiverId, messageText);
        }
    }
}
