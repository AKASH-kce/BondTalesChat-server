namespace BondTalesChat_Server.Interfaces
{
    public interface IGroup
    {
        int GroupId { get; set; }
        string GroupName { get; set; }
        int CreatedBy { get; set; }
        DateTime CreatedAt { get; set; }
    }
}
