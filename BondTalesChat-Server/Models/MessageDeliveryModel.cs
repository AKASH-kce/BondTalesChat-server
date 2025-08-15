using BondTalesChat_Server.Interfaces;
using BondTalesChat_Server.models;

namespace BondTalesChat_Server.Models
{
    public class MessageDeliveryModel : IMessageDelivery
    {
        public int MessageId { get; set; }
        public MessageModel Message { get; set; }

        public int UserId { get; set; }
        public UserModel User { get; set; }

        public byte Status { get; set; } = 0; // 0=Sent,1=Delivered,2=Read
        public DateTime? DeliveredAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
