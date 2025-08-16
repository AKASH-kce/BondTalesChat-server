using BondTalesChat_Server.Interfaces;
using BondTalesChat_Server.Models;
using System.ComponentModel.DataAnnotations;

namespace BondTalesChat_Server.models
{

    public class MessageModel : IMessage
    {
        [Key]  // Primary key
        public int? MessageId { get; set; }
        //public int? MessageId { get; set; }
        public int ConversationId { get; set; }
        //public ConversationModel Conversation { get; set; }

        public int SenderId { get; set; }
        //public UserModel Sender { get; set; }

        public string MessageText { get; set; }
        public string? MediaUrl { get; set; }
        public byte MessageType { get; set; } = 0;
        public DateTime? SentAt { get; set; } = DateTime.UtcNow;
        public bool Edited { get; set; } = false;
        public bool Deleted { get; set; } = false;

        //public ICollection<MessageDeliveryModel> Deliveries { get; set; }
    }
}