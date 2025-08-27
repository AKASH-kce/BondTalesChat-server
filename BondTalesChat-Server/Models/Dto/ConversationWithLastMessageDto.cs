namespace BondTalesChat_Server.Models.Dto
{
    public class ConversationWithLastMessageDto
    {
        public int ConversationId { get; set; }
        public bool IsGroup { get; set; }
        public string Title { get; set; }
        public int? OtherUserId { get; set; }
        public string OtherUserName { get; set; }
        public string OtherUserProfilePicture { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; }
        public int? LastMessageSenderId { get; set; }
        public int UnreadCount { get; set; }
    }
}
