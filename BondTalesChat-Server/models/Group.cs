using BondTalesChat_Server.Interfaces;

namespace BondTalesChat_Server.models
{
    public class Group : IGroup
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
