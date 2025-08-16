using BondTalesChat_Server.models;
using BondTalesChat_Server.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;

namespace BondTalesChat_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageTestController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageTestController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(int ConversationId, int senderId, string Messagetext, string MediaUrl, Byte MessageType, Boolean Edited, Boolean Deleted)
        {
            var msg = await _messageService.SaveAndBroadcastAsync(ConversationId, senderId,  Messagetext,  MediaUrl,  MessageType,  Edited,  Deleted);
            return Ok(new { Status = "Message sent", Message = msg });
        }
    }
}
