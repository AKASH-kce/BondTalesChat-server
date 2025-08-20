using BondTalesChat_Server.models;
using BondTalesChat_Server.Models;
using BondTalesChat_Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace BondTalesChat_Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IConversationService _conversationService;

        public ChatHub(IMessageService messageService, IConversationService conversationService)
        {
            _messageService = messageService;
            _conversationService = conversationService;
        }

        public async Task<MessageModel> SendMessage(int ConversationId, int senderId, string Messagetext, string MediaUrl, byte MessageType, bool Edited, bool Deleted)
        {
            return await _messageService.SaveAndBroadcastAsync(ConversationId, senderId, Messagetext, MediaUrl, MessageType, Edited, Deleted);
        }

        public async Task<MessageModel[]> GetLoginUserAllMessagesByID(int loginUserId)
        {
            var messages = await _messageService.GetMessagesOfCurrentLoginUser(loginUserId);
            return messages.ToArray();

            // OR to send directly to caller:
            // await Clients.Caller.SendAsync("ReceiveAllMessages", messages);
        }

        public async Task<List<UserModel>> GetAllUsers()
        {
            return await _messageService.GetAllUsersChatList();
        }

        // ✅ NEW: Get or create conversation between 2 users
        public async Task<int> GetOrCreateConversation(int currentUserId, int otherUserId)
        {
            try
            {
                return await _conversationService.GetOrCreateConversationAsync(currentUserId, otherUserId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Hub Error] GetOrCreateConversation failed: {ex}");
                throw; // rethrow so client sees disconnect
            }
        }

        // ✅ NEW: Get messages by conversationId
        public async Task<List<MessageModel>> GetMessagesByConversation(int conversationId)
        {
            try
            {
                return await _conversationService.GetMessagesByConversationAsync(conversationId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Hub Error] GetOrCreateConversation failed: {ex}");
                throw; // rethrow so client sees disconnect
            }
        }

        public async Task<int?> getFrndTop1Id(int userId)
        {
            return await _conversationService.GetTopFriendIdAsync(userId);
        }

    }
}
