using System.Data;
using BondTalesChat_Server.Models;
using Microsoft.Data.SqlClient;

namespace BondTalesChat_Server.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public bool UserExists(string email)
        {
            //Console.WriteLine($"the mail that came here {email}.");
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var checkCmd = new SqlCommand(
                    "SELECT 1 FROM Users WHERE email = @e", conn))
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
                    "INSERT INTO Users (username, email, userpassword, phoneNumber) VALUES (@u, @e, @h, @p)", conn))
                {
                    insertCmd.Parameters.AddWithValue("@u", user.username);
                    insertCmd.Parameters.AddWithValue("@e", user.email);
                    insertCmd.Parameters.AddWithValue("@h", user.password);
                    insertCmd.Parameters.AddWithValue("@p", user.phoneNumber);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        public UserModel? GetUserByEmail(string email)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT UserId, username, email, userpassword, ProfilePicture, CreatedAt, phoneNumber FROM Users Where email = @e", conn))
                {
                    cmd.Parameters.AddWithValue("@e", email);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserModel
                            {
                                UserId = reader.GetInt32(0),
                                username = reader.GetString(1),
                                email = reader.GetString(2),
                                password = reader.GetString(3),
                                ProfilePicture = "No picture.",
                                CreatedAt = reader.GetDateTime(5),
                                phoneNumber = reader.GetString(6)
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
