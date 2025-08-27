using BondTalesChat_Server.models;
using BondTalesChat_Server.Models.Dto;
using Microsoft.Data.SqlClient;

namespace BondTalesChat_Server.Services
{
    public interface IConversationService
    {
        Task<int> GetOrCreateConversationAsync(int currentUserId, int otherUserId);
        Task<List<MessageModel>> GetMessagesByConversationAsync(int conversationId);

        Task<int?> GetTopFriendIdAsync(int userId);

        Task<List<ConversationWithLastMessageDto>> GetUserConversationsWithLastMessageAsync(int userId);
    }

    public class ConversationService : IConversationService
    {
        private readonly string _connectionString;

        public ConversationService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<int> GetOrCreateConversationAsync(int currentUserId, int otherUserId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string checkQuery = @"
                SELECT c.ConversationId 
                FROM Conversations c
                INNER JOIN ConversationMembers cm1 ON c.ConversationId = cm1.ConversationId
                INNER JOIN ConversationMembers cm2 ON c.ConversationId = cm2.ConversationId
                WHERE c.IsGroup = 0
                  AND cm1.UserId = @currentUserId
                  AND cm2.UserId = @otherUserId";

                using (SqlCommand cmd = new SqlCommand(checkQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@currentUserId", currentUserId);
                    cmd.Parameters.AddWithValue("@otherUserId", otherUserId);

                    var existingConvId = await cmd.ExecuteScalarAsync();
                    if (existingConvId != null)
                        return Convert.ToInt32(existingConvId);
                }

                // Create new conversation
                int newConversationId;
                string insertConv = @"
                INSERT INTO Conversations (IsGroup, CreatedBy, CreatedAt)
                OUTPUT INSERTED.ConversationId
                VALUES (0, @currentUserId, SYSUTCDATETIME())";

                using (SqlCommand cmd = new SqlCommand(insertConv, conn))
                {
                    cmd.Parameters.AddWithValue("@currentUserId", currentUserId);
                    newConversationId = (int)await cmd.ExecuteScalarAsync();
                }

                // Add members
                string insertMembers = @"
                INSERT INTO ConversationMembers (ConversationId, UserId)
                VALUES (@convId, @userId1), (@convId, @userId2)";

                using (SqlCommand cmd = new SqlCommand(insertMembers, conn))
                {
                    cmd.Parameters.AddWithValue("@convId", newConversationId);
                    cmd.Parameters.AddWithValue("@userId1", currentUserId);
                    cmd.Parameters.AddWithValue("@userId2", otherUserId);
                    await cmd.ExecuteNonQueryAsync();
                }

                return newConversationId;
            }
        }

        public async Task<List<MessageModel>> GetMessagesByConversationAsync(int conversationId)
        {
            var messages = new List<MessageModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"
                SELECT MessageId, ConversationId, SenderId, MessageText, MediaUrl, MessageType, SentAt, Edited, Deleted
                FROM Messages
                WHERE ConversationId = @conversationId
                ORDER BY SentAt ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@conversationId", conversationId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            messages.Add(new MessageModel
                            {
                                MessageId = reader.GetInt32(0),
                                ConversationId = reader.GetInt32(1),
                                SenderId = reader.GetInt32(2),
                                MessageText = reader.IsDBNull(3) ? null : reader.GetString(3),
                                MediaUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                                MessageType = reader.GetByte(5),
                                SentAt = reader.GetDateTime(6),
                                Edited = reader.GetBoolean(7),
                                Deleted = reader.GetBoolean(8)
                            });
                        }
                    }
                }
            }

            return messages;
        }


        public async Task<int?> GetTopFriendIdAsync(int userId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"
            SELECT TOP 1 
                   CASE 
                        WHEN cm.UserId = @userId THEN cm2.UserId 
                        ELSE cm.UserId 
                   END AS FriendId
            FROM Conversations c
            INNER JOIN ConversationMembers cm ON c.ConversationId = cm.ConversationId
            INNER JOIN ConversationMembers cm2 ON c.ConversationId = cm2.ConversationId
            INNER JOIN Messages m ON c.ConversationId = m.ConversationId
            WHERE c.IsGroup = 0
              AND (cm.UserId = @userId OR cm2.UserId = @userId)
            ORDER BY m.SentAt DESC;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    var result = await cmd.ExecuteScalarAsync();
                    return result == null || result == DBNull.Value ? null : (int?)Convert.ToInt32(result);
                }
            }
        }

        public async Task<List<ConversationWithLastMessageDto>> GetUserConversationsWithLastMessageAsync(int userId)
        {
            var conversations = new List<ConversationWithLastMessageDto>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"
            SELECT 
                c.ConversationId,
                c.IsGroup,
                c.Title,
                u.UserId,
                u.username,
                u.ProfilePicture,
                m.MessageText,
                m.SentAt,
                m.SenderId,
                (SELECT COUNT(*) FROM MessageDeliveries md 
                 WHERE md.MessageId = m.MessageId AND md.UserId = @userId AND md.Status = 0) as UnreadCount
            FROM Conversations c
            INNER JOIN ConversationMembers cm ON c.ConversationId = cm.ConversationId
            LEFT JOIN (
                SELECT ConversationId, MAX(SentAt) as LatestSentAt
                FROM Messages
                GROUP BY ConversationId
            ) lm ON c.ConversationId = lm.ConversationId
            LEFT JOIN Messages m ON lm.ConversationId = m.ConversationId AND lm.LatestSentAt = m.SentAt
            LEFT JOIN ConversationMembers cm2 ON c.ConversationId = cm2.ConversationId AND cm2.UserId != @userId
            LEFT JOIN Users u ON cm2.UserId = u.UserId
            WHERE cm.UserId = @userId
            ORDER BY m.SentAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var conversation = new ConversationWithLastMessageDto
                            {
                                ConversationId = reader.GetInt32(0),
                                IsGroup = reader.GetBoolean(1),
                                Title = reader.IsDBNull(2) ? null : reader.GetString(2),
                                OtherUserId = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                OtherUserName = reader.IsDBNull(4) ? null : reader.GetString(4),
                                OtherUserProfilePicture = reader.IsDBNull(5) ? null : reader.GetString(5),
                                LastMessage = reader.IsDBNull(6) ? null : reader.GetString(6),
                                LastMessageTime = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7),
                                LastMessageSenderId = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8),
                                UnreadCount = reader.IsDBNull(9) ? 0 : reader.GetInt32(9)
                            };

                            conversations.Add(conversation);
                        }
                    }
                }
            }

            return conversations;
        }


    }

}