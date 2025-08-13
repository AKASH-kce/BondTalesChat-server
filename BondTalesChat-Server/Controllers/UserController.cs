using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using BCrypt.Net;

namespace BondTalesChat_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] JsonElement  body)
        {
            if (!body.TryGetProperty("username", out var uProp) ||
                !body.TryGetProperty("email", out var eProp) ||
                !body.TryGetProperty("password", out var pProp))
                return BadRequest(new { error = "username and password are required" });

            var username = uProp.GetString();
            var email = eProp.GetString();
            var password = pProp.GetString();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||  string.IsNullOrWhiteSpace(password))
                return BadRequest(new { error = "username and password cannot be empty" });

            // Check if user exists
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (var checkCmd = new SqlCommand(
                    "SELECT 1 FROM Users WHERE Email = @e", conn))
                {
                   // checkCmd.Parameters.AddWithValue("@u", username!);
                    checkCmd.Parameters.AddWithValue("@e", email!);
                    var exists = checkCmd.ExecuteScalar();
                    if (exists != null)
                        return BadRequest(new { error = "User already exists" });
                }

                // Hash password
                var hash = BCrypt.Net.BCrypt.HashPassword(password);

                using (var insertCmd = new SqlCommand(
                    "INSERT INTO Users (Username, Email, PasswordHash) VALUES (@u, @e, @h)", conn))
                {
                    insertCmd.Parameters.AddWithValue("@u", username!);
                    insertCmd.Parameters.AddWithValue("@h", hash);
                    insertCmd.Parameters.AddWithValue("@e", email!);
                    insertCmd.ExecuteNonQuery();
                }
            }

            return Ok(new { message = "User registered successfully" });
        }
    }
}
