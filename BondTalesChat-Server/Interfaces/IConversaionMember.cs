namespace BondTalesChat_Server.Interfaces
{
    public interface IConversationMember
    {
        int ConversationId { get; set; }
        int UserId { get; set; }
        DateTime JoinedAt { get; set; }
    }
}
