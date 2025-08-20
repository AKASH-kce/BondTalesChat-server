using BondTalesChat_Server.Hubs;
using BondTalesChat_Server.models;
using BondTalesChat_Server.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BondTalesChat_Server.Services
{
    public interface IMessageService
    {
        Task<MessageModel> SaveAndBroadcastAsync(int ConversationId, int senderId, string Messagetext, string MediaUrl, byte MessageType, bool Edited, bool Deleted);
        Task<List<MessageModel>> GetMessagesOfCurrentLoginUser(int loginuserId);
        Task<List<UserModel>> GetAllUsersChatList();
    }

    public class MessageService : IMessageService
    {
        private readonly IHubContext<ChatHub> _hub;
        private readonly string _connectionString;

        public MessageService(IHubContext<ChatHub> hubContext, IConfiguration configuration)
        {
            _hub = hubContext;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<MessageModel> SaveAndBroadcastAsync(int ConversationId, int senderId, string Messagetext, string MediaUrl, byte MessageType, bool Edited, bool Deleted)
        {
            var msg = new MessageModel
            {
                ConversationId = ConversationId,
                SenderId = senderId,
                MessageText = Messagetext,
                MediaUrl = MediaUrl,
                MessageType = MessageType,
                Edited = Edited,
                Deleted = Deleted,
                SentAt = DateTime.UtcNow
            };

            await using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();

                var query = @"INSERT INTO Messages 
                                (ConversationId, SenderId, MessageText, MediaUrl, MessageType, Edited, Deleted, SentAt)
                              OUTPUT INSERTED.MessageId
                              VALUES (@ConversationId, @SenderId, @MessageText, @MediaUrl, @MessageType, @Edited, @Deleted, @SentAt)";

                await using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ConversationId", msg.ConversationId);
                    cmd.Parameters.AddWithValue("@SenderId", msg.SenderId);
                    cmd.Parameters.AddWithValue("@MessageText", (object?)msg.MessageText ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MediaUrl", (object?)msg.MediaUrl ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MessageType", msg.MessageType);
                    cmd.Parameters.AddWithValue("@Edited", msg.Edited);
                    cmd.Parameters.AddWithValue("@Deleted", msg.Deleted);
                    cmd.Parameters.AddWithValue("@SentAt", msg.SentAt);

                    msg.MessageId = (int)await cmd.ExecuteScalarAsync();
                }
            }

            // Broadcast via SignalR
            await _hub.Clients.All.SendAsync("ReceiveMessage", msg);
            return msg;
        }

        public async Task<List<MessageModel>> GetMessagesOfCurrentLoginUser(int loginuserId)
        {
            var messages = new List<MessageModel>();

            await using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();

                var query = @"SELECT m.MessageId, m.ConversationId, m.SenderId, m.MessageText, 
                                     m.MediaUrl, m.MessageType, m.SentAt, m.Edited, m.Deleted
                              FROM Messages m
                              INNER JOIN ConversationMembers cm ON m.ConversationId = cm.ConversationId
                              WHERE cm.UserId = @UserId
                              ORDER BY m.SentAt ASC";

                await using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserId", loginuserId);

                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        messages.Add(new MessageModel
                        {
                            MessageId = reader.GetInt32(0),
                            ConversationId = reader.GetInt32(1),
                            SenderId = reader.GetInt32(2),
                            MessageText = reader.IsDBNull(3) ? null : reader.GetString(3),
                            MediaUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                            MessageType = (byte)reader.GetByte(5),
                            SentAt = reader.GetDateTime(6),
                            Edited = reader.GetBoolean(7),
                            Deleted = reader.GetBoolean(8)
                        });
                    }
                }
            }

            return messages;
        }

        public async Task<List<UserModel>> GetAllUsersChatList()
        {
            var users = new List<UserModel>();

            await using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();

                var query = "SELECT UserId, username, email, userpassword, ProfilePicture, CreatedAt, phoneNumber FROM Users";

                await using (var cmd = new SqlCommand(query, con))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new UserModel
                        {
                            UserId = reader.GetInt32(0),
                            username = reader.GetString(1),
                            email = reader.GetString(2),
                            password = reader.GetString(3),
                            ProfilePicture = reader.IsDBNull(4) ? null : reader.GetString(4),
                            CreatedAt = reader.GetDateTime(5),
                            phoneNumber = reader.IsDBNull(6) ? null : reader.GetString(6)
                        });
                    }
                }
            }

            return users;
        }
    }
}
