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
                return BadRequest(new { success = false, message = "username, email, and password cannot be empty" });
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
                password = hash
            };

            _userRepository.CreateUser(user);

            return Ok(new { success = true, message = "User registered successfully" });
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

            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"]))
            });


            return Ok(new
            {
                success = true,
                message = "Login successful",
                token,
                user = new
                {
                    user.userId,
                    user.username,
                    user.email
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

            return Ok(new
            {
                success = true,
                user = new
                {
                    id = int.Parse(userId),
                    username,
                    email
                }
            });
        }
    }
}
