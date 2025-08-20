using BondTalesChat_Server.Interfaces;
using BondTalesChat_Server.models;
using System.ComponentModel.DataAnnotations;

namespace BondTalesChat_Server.Models
{
    public class ConversationModel : IConversation
    {
        [Key]
        public int ConversationId { get; set; }
        public bool IsGroup { get; set; } = false;
        public string Title { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ConversationMemberModel> Members { get; set; }
        public ICollection<MessageModel> Messages { get; set; }
    }
}
