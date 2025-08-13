namespace BondTalesChat_Server.Interfaces
{
    public interface IMessage
    {
        int MessageId { get; set; }
        int SenderId { get; set; }
        int? GroupId { get; set; }
        int? ReceiverId { get; set; }
        string MessageText { get; set; }
        DateTime SentAt { get; set; }
    }

}
