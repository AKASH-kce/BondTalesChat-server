namespace BondTalesChat_Server.Interfaces
{
    public interface IUserConnection
    {
        string ConnectionId { get; set; }
        int UserId { get; set; }
        DateTime ConnectedAt { get; set; }
    }
}
