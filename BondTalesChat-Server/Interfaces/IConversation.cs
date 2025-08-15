namespace BondTalesChat_Server.Interfaces
{
    public interface IConversation
    {
        int ConversationId { get; set; }
        bool IsGroup { get; set; }
        string Title { get; set; }
        int? CreatedBy { get; set; }
    }
}
