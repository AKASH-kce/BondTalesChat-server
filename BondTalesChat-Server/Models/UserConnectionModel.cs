using BondTalesChat_Server.Interfaces;

namespace BondTalesChat_Server.Models
{
    public class UserConnectionModel : IUserConnection
    {
        public string ConnectionId { get; set; }
        public int UserId { get; set; }
        public UserModel User { get; set; }
        public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
    }
}
