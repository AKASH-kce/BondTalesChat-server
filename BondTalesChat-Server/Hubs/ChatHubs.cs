using BondTalesChat_Server.Data;
using BondTalesChat_Server.models;
using Microsoft.AspNetCore.SignalR;

namespace BondTalesChat_Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(int messageId, int senderId, int groupId, int receiverId, string messageText, DateTime sentAt)
        {
            var msg = new Message
            {
                MessageId = messageId,
                SenderId = senderId,
                GroupId = groupId,
                ReceiverId = receiverId,
                MessageText = messageText,
                SentAt = sentAt
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessage", senderId, receiverId, messageText, sentAt);
        }


    }
}
