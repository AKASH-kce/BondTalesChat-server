using BondTalesChat_Server.Interfaces;

namespace BondTalesChat_Server.Models
{
    public class UserModel: IUser
    {
        public int userId { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }

    }
}
