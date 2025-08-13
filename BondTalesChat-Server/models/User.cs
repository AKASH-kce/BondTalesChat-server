using BondTalesChat_Server.Interfaces;

namespace BondTalesChat_Server.models
{
    public class User : IUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
