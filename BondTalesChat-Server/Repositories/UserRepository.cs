using BondTalesChat_Server.Models;
using Microsoft.Data.SqlClient;

namespace BondTalesChat_Server.Repositories
{
    public interface IUserRepository 
    {
   

    }

    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public bool UserExists(string email)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var checkCmd = new SqlCommand(
                    "SELECT 1 FROM Users WHERE Email = @e", conn))
                {
                    checkCmd.Parameters.AddWithValue("@e", email);
                    var exists = checkCmd.ExecuteScalar();
                    return exists != null;
                }
            }
        }

        public void CreateUser(UserModel user)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var insertCmd = new SqlCommand(
                    "INSERT INTO Users (Username, Email, PasswordHash) VALUES (@u, @e, @h)", conn))
                {
                    insertCmd.Parameters.AddWithValue("@u", user.username);
                    insertCmd.Parameters.AddWithValue("@e", user.email);
                    insertCmd.Parameters.AddWithValue("@h", user.password);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }
    }
}
