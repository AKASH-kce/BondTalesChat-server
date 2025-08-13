using System.Text.Json;
using BCrypt.Net;
using BondTalesChat_Server.Models;
using BondTalesChat_Server.Models.Dto;
using BondTalesChat_Server.Repositories;
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

        public UserController(IConfiguration configuration)
        {
            _userRepository = new UserRepository(configuration);
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
    }
}
