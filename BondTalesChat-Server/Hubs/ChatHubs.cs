using BondTalesChat_Server.models;
using BondTalesChat_Server.Models;
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

        public async Task<MessageModel> SendMessage(int ConversationId, int senderId, string Messagetext, string MediaUrl, byte MessageType, bool Edited, bool Deleted)
        {
            return await _messageService.SaveAndBroadcastAsync(ConversationId, senderId, Messagetext, MediaUrl, MessageType, Edited, Deleted);
        }

        public async Task<MessageModel[]> GetLoginUserAllMessagesByID(int loginUserId)
        {
            var messages = await _messageService.GetMessagesOfCurrentLoginUser(loginUserId);
            return messages;

            // OR to send directly to caller:
            // await Clients.Caller.SendAsync("ReceiveAllMessages", messages);
        }

        public async Task<List<UserModel>> GetAllUsers()
        {
            return await _messageService.GetAllUsersChatList();
        }
    }
}
