using BondTalesChat_Server.Data;
using BondTalesChat_Server.Hubs;
using BondTalesChat_Server.models;
using Microsoft.AspNetCore.SignalR;

namespace BondTalesChat_Server.Services
{
    public interface IMessageService
    {
        Task<Message> SaveAndBroadcastAsync(int senderId, int? groupId, int? receiverId, string messageText);
    }

    public class MessageService : IMessageService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageService(AppDbContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<Message> SaveAndBroadcastAsync(int senderId, int? groupId, int? receiverId, string messageText)
        {
            var msg = new Message
            {
                SenderId = senderId,
                GroupId = groupId == 0 ? null : groupId,
                ReceiverId = receiverId == 0 ? null : receiverId,
                MessageText = messageText,
                SentAt = DateTime.Now
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", msg);
            return msg;
        }
    }
}
