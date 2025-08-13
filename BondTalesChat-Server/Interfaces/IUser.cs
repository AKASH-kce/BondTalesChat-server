namespace BondTalesChat_Server.Interfaces
{
    public interface IUser
    {
        int userId { get; set; }
        string username { get; set; }
        string email { get; set; }
        string password { get; set; }
    }
}
