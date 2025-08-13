namespace BondTalesChat_Server.Interfaces
{
    public interface IUser
    {
        int UserId { get; set; }
        string UserName { get; set; }
        string Email { get; set; }
        string PasswordHash { get; set; }
    }
}
