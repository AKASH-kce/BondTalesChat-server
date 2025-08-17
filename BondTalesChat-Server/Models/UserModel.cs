using BondTalesChat_Server.Interfaces;
using BondTalesChat_Server.models;
using System;
using System.Collections.Generic;

namespace BondTalesChat_Server.Models
{
    public class UserModel : IUser
    {
        public int UserId { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string phoneNumber { get; set; }

        //public ICollection<ConversationMemberModel> ConversationMembers { get; set; }
        //public ICollection<MessageModel> MessagesSent { get; set; }
        //public ICollection<UserConnectionModel> Connections { get; set; }
    }
}
