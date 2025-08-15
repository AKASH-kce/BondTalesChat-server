using BondTalesChat_Server.Interfaces;

namespace BondTalesChat_Server.Models
{
    public class ConversationMemberModel : IConversationMember
    {
        public int ConversationId { get; set; }
        public ConversationModel Conversation { get; set; }

        public int UserId { get; set; }
        public UserModel User { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
