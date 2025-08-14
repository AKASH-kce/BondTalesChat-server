using BondTalesChat_Server.Interfaces;

namespace BondTalesChat_Server.models
{

    public class Message : IMessage
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int? GroupId { get; set; }
        public int? ReceiverId { get; set; }
        public string MessageText { get; set; }
        public DateTime SentAt { get; set; }
    }
}