namespace BondTalesChat_Server.Interfaces
{
    public interface IGroupMember
    {
        int GroupId { get; set; }
        int UserId { get; set; }
    }
}
