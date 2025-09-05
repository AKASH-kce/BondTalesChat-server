using System.Data;
using BondTalesChat_Server.Models;
using Npgsql;

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
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var checkCmd = new NpgsqlCommand(
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
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var insertCmd = new NpgsqlCommand(
                    "INSERT INTO Users (username, email, userpassword, phoneNumber) VALUES (@u, @e, @h, @p)", conn))
                {
                    insertCmd.Parameters.AddWithValue("@u", user.username);
                    insertCmd.Parameters.AddWithValue("@e", user.email);
                    insertCmd.Parameters.AddWithValue("@h", user.password);
                    insertCmd.Parameters.AddWithValue("@p", user.phoneNumber);
                    insertCmd.ExecuteNonQuery();
                    // Get the auto-generated UserId
                    //var newId = (int)(decimal)insertCmd.ExecuteScalar(); // SCOPE_IDENTITY() returns decimal
                    //user.UserId = newId;
                    //return user;
                }
            }
        }

        public UserModel? GetUserByEmail(string email)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT UserId, username, email, userpassword, ProfilePicture, CreatedAt, phoneNumber FROM Users Where email = @e", conn))
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
                                ProfilePicture = reader.IsDBNull(4) ? "No Picture" : reader.GetString(4),
                                CreatedAt = reader.GetDateTime(5),
                                phoneNumber = reader.GetString(6)
                            };
                        }
                    }
                }
            }
            return null;
        }

        // Repositories/UserRepository.cs

        public bool UpdateUser(int userId, string username, string email, string? phoneNumber, string passwordHash)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
            UPDATE Users 
            SET username = @u, 
                email = @e, 
                phoneNumber = @p, 
                userpassword = @h 
            WHERE UserId = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@p", (object)phoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@h", passwordHash);
                    cmd.Parameters.AddWithValue("@id", userId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Optional: Add method to get user by ID
        public UserModel? GetUserById(int userId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"
            SELECT UserId, username, email, userpassword, ProfilePicture, CreatedAt, phoneNumber 
            FROM Users 
            WHERE UserId = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
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
                                ProfilePicture = reader.IsDBNull(4) ? null : reader.GetString(4),
                                CreatedAt = reader.GetDateTime(5),
                                phoneNumber = reader.IsDBNull(6) ? null : reader.GetString(6)
                            };
                        }
                    }
                }
            }
            return null;
        }

        // UserRepository.cs

        public bool UpdateProfilePicture(int userId, string profilePictureUrl)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "UPDATE Users SET ProfilePicture = @p WHERE UserId = @id", conn))
                {
                    // cmd.Parameters.AddWithValue("@p", profilePictureUrl);

                    // ✅ Properly handle null
                    cmd.Parameters.Add("@p", NpgsqlTypes.NpgsqlDbType.Varchar, 500).Value =
                        (object)profilePictureUrl ?? DBNull.Value;
                    cmd.Parameters.AddWithValue("@id", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdatePassword(int userId, string password)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using(var cmd = new NpgsqlCommand("UPDATE Users SET userpassword = @h WHERE UserId = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@h",password);
                    cmd.Parameters.AddWithValue("@id", userId);
                    return cmd.ExecuteNonQuery() > 0;
                }

            }

        }
    }
}
