using System.Security.Claims;
using System.Text.Json;
using BCrypt.Net;
using BondTalesChat_Server.Models;
using BondTalesChat_Server.Models.Dto;
using BondTalesChat_Server.Repositories;
using BondTalesChat_Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BondTalesChat_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _config;
        public UserController(IConfiguration configuration)
        {
            _userRepository = new UserRepository(configuration);
            _jwtService = new JwtService(configuration);
            _config = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] registrationDto  userDetails)
        {

            if (string.IsNullOrWhiteSpace(userDetails.Username) || string.IsNullOrWhiteSpace(userDetails.Email) || string.IsNullOrWhiteSpace(userDetails.Password))
            {
                return BadRequest(new { success = false, message = "username, email, phoneNumber and password cannot be empty" });
            }

            if (_userRepository.UserExists(userDetails.Email))
            {
                return BadRequest(new { success = false, message = "User already exists" });
            }

            var hash = BCrypt.Net.BCrypt.HashPassword(userDetails.Password);

            var user = new UserModel
            {
                username = userDetails.Username!,
                email = userDetails.Email!,
                password = hash,
                phoneNumber = userDetails.PhoneNumber
            };

            _userRepository.CreateUser(user);

            var userData = _userRepository.GetUserByEmail(userDetails.Email);

            if (userData == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "User data could not be retrieved after registration." });
            }

            var token = _jwtService.GenerateToken(userData);

            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // ← SET THIS TO FALSE FOR LOCALHOST
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"]))
            });

            return Ok(new
            {
                success = true,
                message = "User registered successfully",
                token,
                user = new
                {
                    userData.UserId,
                    userData.username,
                    userData.email,
                    userData.phoneNumber,
                    userData.ProfilePicture
                }
            });
        }

        [HttpPost("login")]

        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            if(string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest(new {success = false, message = "Email and password cannot be empty."});
            }

           
            var user = _userRepository.GetUserByEmail(loginDto.Email);

            if (user == null)
            {
                
                return BadRequest(new { success = false, message = "Create an account to Login." });
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.password))
            {
                return BadRequest(new { success = false, message = "Invalid credentials." });
            }

            var token = _jwtService.GenerateToken(user);
            // var token = GenerateJwtToken(user);

            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // ← SET THIS TO FALSE FOR LOCALHOST
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"]))
            });


            return Ok(new
            {
                success = true,
                message = "Login successful",
                token,
                user = new
                {
                    user.UserId,
                    user.username,
                    user.email,
                    user.phoneNumber,
                    user.ProfilePicture
                }
            });
        }

        //Logout

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Append("token", "", new CookieOptions
            { HttpOnly = true,
              Secure = true,
              SameSite = SameSiteMode.None,
              Expires = DateTime.UtcNow.AddYears(-1) // Expire immediately
            });

            return Ok(new { success = true, message = "Logged out successfully."});

        }

        //cookie related change.

        [HttpGet("verify-auth")]
        [Authorize] // This ensures only authenticated requests pass
        public IActionResult VerifyAuth()
        {
            // Get user info from the token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var phoneNumber = User.FindFirst(ClaimTypes.MobilePhone)?.Value;
            var profilePicture = User.FindFirst("ProfilePicture")?.Value;
            Console.WriteLine(profilePicture);
            return Ok(new
            {
                success = true,
                message = "verified",
                user = new
                {
                    userId,
                    username,
                    email,
                    phoneNumber,
                    profilePicture
                }
            });
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
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"]))
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


        // UserController.cs

        [HttpPut("update-profile-picture")]
        [Authorize]
        public IActionResult UpdateProfilePicture([FromBody] UpdateProfilePictureDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { success = false, message = "Invalid token." });
            }

            //if (string.IsNullOrWhiteSpace(dto.ProfilePictureUrl))
            //{
            //    return BadRequest(new { success = false, message = "Image URL is required." });
            //}

            // Validate URL format (optional)
            //if (!Uri.IsWellFormedUriString(dto.ProfilePictureUrl, UriKind.Absolute))
            //{
            //    return BadRequest(new { success = false, message = "Invalid image URL." });
            //}

            // ✅ Allow null (for removal)
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
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"]))
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
