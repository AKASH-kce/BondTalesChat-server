namespace BondTalesChat_Server.Interfaces
{
    public interface IMessage
    {
        //int ?MessageId { get; set; }
        int ConversationId { get; set; }
        int SenderId { get; set; }
        string MessageText { get; set; }
        string? MediaUrl { get; set; }
        byte MessageType { get; set; }
        DateTime? SentAt { get; set; }
        bool Edited { get; set; }
        bool Deleted { get; set; }
    }

}
