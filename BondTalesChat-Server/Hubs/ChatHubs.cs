using BondTalesChat_Server.Data;
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

        public async Task SendMessage(int senderId, int receiverId,string message)
        {
            var msg = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message,
                SentAt = DateTime.Now
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("Receive message", senderId, receiverId, message, msg.SentAt);


        }
    }
}
