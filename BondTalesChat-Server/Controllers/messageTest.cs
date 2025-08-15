using BondTalesChat_Server.models;
using BondTalesChat_Server.Services;
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
        public async Task<IActionResult> SendMessage(int senderId, int groupId, int receiverId, string messageText)
        {
            var msg = await _messageService.SaveAndBroadcastAsync(senderId, groupId, receiverId, messageText);
            return Ok(new { Status = "Message sent", Message = msg });
        }
    }
}
