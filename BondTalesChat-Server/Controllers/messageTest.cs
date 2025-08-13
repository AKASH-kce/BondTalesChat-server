using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using BondTalesChat_Server.Hubs;
using BondTalesChat_Server.Data;

namespace BondTalesChat_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageTestController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly AppDbContext _context;

        public MessageTestController(IHubContext<ChatHub> hubContext, AppDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        // POST: api/MessageTest/send
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(int senderId, int receiverId, string message)
        {
            // Save message to DB
            var msg = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message,
                SentAt = DateTime.Now
            };
            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            // Send message via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", senderId, receiverId, message, msg.SentAt);

            return Ok(new { Status = "Message sent", Message = msg });
        }
    }
}
