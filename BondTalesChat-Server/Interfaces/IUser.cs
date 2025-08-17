namespace BondTalesChat_Server.Interfaces
{
    public interface IUser
    {
        int UserId { get; set; }
        string username { get; set; }
        string email { get; set; }
        string password { get; set; }
        string ProfilePicture { get; set; }
        DateTime CreatedAt { get; set; }
        string phoneNumber { get; set; }
    }
}
