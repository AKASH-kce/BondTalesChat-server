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

        public async Task<MessageModel> SendMessage(int ConversationId, int senderId, string Messagetext, string MediaUrl, Byte MessageType, Boolean Edited, Boolean Deleted)
        {
            return await _messageService.SaveAndBroadcastAsync(ConversationId,  senderId,  Messagetext,  MediaUrl, MessageType,  Edited,  Deleted);
        }
    }
}
