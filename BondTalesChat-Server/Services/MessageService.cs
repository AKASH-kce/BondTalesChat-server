using BondTalesChat_Server.Data;
using BondTalesChat_Server.Hubs;
using BondTalesChat_Server.models;
using Microsoft.AspNetCore.SignalR;

namespace BondTalesChat_Server.Services
{
    public interface IMessageService
    {
        Task<MessageModel> SaveAndBroadcastAsync(int ConversationId, int senderId, string Messagetext, string MediaUrl, byte MessageType, bool Edited, bool Deleted);
    }

    public class MessageService : IMessageService
    {
        private readonly AppDbContext _db;
        private readonly IHubContext<ChatHub> _hub;

        public MessageService(AppDbContext context, IHubContext<ChatHub> hubContext)
        {
            _db = context;
            _hub = hubContext;
        }

        public async Task<MessageModel> SaveAndBroadcastAsync(int ConversationId, int senderId, string Messagetext, string MediaUrl, byte MessageType, bool Edited, bool Deleted)
        {
            var msg = new MessageModel
            {
                ConversationId = ConversationId,
                SenderId = senderId,
                MessageText = Messagetext,
                MediaUrl = MediaUrl,
                MessageType = MessageType,
                Edited = Edited,
                Deleted = Deleted
            };

            _db.Messages.Add(msg);
            await _db.SaveChangesAsync(); // EF automatically sets MessageId after this

            await _hub.Clients.All.SendAsync("ReceiveMessage", msg);
            return msg;
        }
    }
}
