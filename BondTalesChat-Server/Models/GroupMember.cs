using BondTalesChat_Server.Interfaces;

namespace BondTalesChat_Server.models
{
    public class GroupMember : IGroupMember
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
    }
}
