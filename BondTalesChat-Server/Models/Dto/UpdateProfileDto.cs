using System.ComponentModel.DataAnnotations;

namespace BondTalesChat_Server.Models.Dto
{
    public class UpdateProfileDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
