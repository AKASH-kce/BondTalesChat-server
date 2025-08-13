using BondTalesChat_Server.Data;
using BondTalesChat_Server.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

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
        public async Task<IActionResult> SendMessage(int MessageId, int SenderId, int GroupId,int ReceiverId,string MessageText,DateTime SentAt)
        {
            // Save message to DB
            var msg = new Message
            {
                MessageId = MessageId,
                SenderId = SenderId,
                GroupId = GroupId,
                ReceiverId = ReceiverId,
                MessageText = MessageText,
                SentAt = SentAt
            };
            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            // Send message via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", msg.MessageId, msg.SenderId, msg.GroupId, msg.ReceiverId,msg.MessageText,msg.SentAt);

            return Ok(new { Status = "Message sent", Message = msg });
        }
    }
}
