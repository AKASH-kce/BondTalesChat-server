using System.Security.Claims;
using BondTalesChat_Server.Models.Dto;
using BondTalesChat_Server.Repositories;
using BondTalesChat_Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BondTalesChat_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _configuration;

        public UserProfileController(IConfiguration configuration)
        {
            _userRepository = new UserRepository(configuration);
            _jwtService = new JwtService(configuration);
            _configuration = configuration;
        }

        // UserController.cs

        [HttpPut("update")]
        [Authorize]
        public IActionResult UpdateProfile([FromBody] UpdateProfileDto updateDto)
        {
            // Extract user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine("This is the userId: koushiik " + userIdClaim);
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { success = false, message = "Invalid authentication token." });
            }

            // Fetch current user from DB
            Console.WriteLine("This is the userID which I got" + userId);
            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return NotFound(new { success = false, message = "User not found." });
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(updateDto.CurrentPassword, user.password))
            {
                return BadRequest(new { success = false, message = "Current password is incorrect." });
            }

            // Hash new password if provided
            string hashedPassword = user.password;
            if (!string.IsNullOrWhiteSpace(updateDto.NewPassword))
            {
                hashedPassword = BCrypt.Net.BCrypt.HashPassword(updateDto.NewPassword);
            }

            // Update user in database
            bool updated = _userRepository.UpdateUser(
                userId: userId,
                username: updateDto.Username!,
                email: updateDto.Email!,
                phoneNumber: updateDto.PhoneNumber,
                passwordHash: hashedPassword
            );

            if (!updated)
            {
                return StatusCode(500, new { success = false, message = "Failed to update user. Please try again." });
            }

            // Fetch updated user data
            var updatedUser = _userRepository.GetUserById(userId);
            if (updatedUser == null)
            {
                return StatusCode(500, new { success = false, message = "User updated, but could not reload data." });
            }

            // Generate new JWT token (with updated claims)
            var newToken = _jwtService.GenerateToken(updatedUser);

            // Refresh the authentication cookie
            Response.Cookies.Append("token", newToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Set to true in production with HTTPS
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"]))
            });

            // Return success response
            return Ok(new
            {
                success = true,
                message = "Profile updated successfully.",
                token = newToken,
                user = new
                {
                    updatedUser.UserId,
                    updatedUser.username,
                    updatedUser.email,
                    updatedUser.phoneNumber,
                    updatedUser.ProfilePicture
                }
            });
        }

        [HttpPut("update-profile-picture")]
        [Authorize]
        public IActionResult UpdateProfilePicture([FromBody] UpdateProfilePictureDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { success = false, message = "Invalid token." });
            }

            // Only validate format if URL is provided
            if (!string.IsNullOrWhiteSpace(dto.ProfilePictureUrl))
            {
                if (!Uri.IsWellFormedUriString(dto.ProfilePictureUrl, UriKind.Absolute))
                {
                    return BadRequest(new { success = false, message = "Invalid image URL format." });
                }
            }

            // Update only the profile picture
            bool updated = _userRepository.UpdateProfilePicture(userId, dto.ProfilePictureUrl);
            if (!updated)
            {
                return StatusCode(500, new { success = false, message = "Failed to update profile picture." });
            }

            // Reload updated user
            Console.WriteLine("This is the userID which I got" + userId);
            var updatedUser = _userRepository.GetUserById(userId);
            if (updatedUser == null)
            {
                return StatusCode(500, new { success = false, message = "User not found after update." });
            }

            // Generate new token with updated picture
            var newToken = _jwtService.GenerateToken(updatedUser);

            // Refresh auth cookie
            Response.Cookies.Append("token", newToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"]))
            });

            return Ok(new
            {
                success = true,
                message = "Profile picture updated successfully.",
                token = newToken,
                user = new
                {
                    id = updatedUser.UserId,
                    updatedUser.username,
                    updatedUser.email,
                    updatedUser.phoneNumber,
                    updatedUser.ProfilePicture
                }
            });
        }
    }
}
