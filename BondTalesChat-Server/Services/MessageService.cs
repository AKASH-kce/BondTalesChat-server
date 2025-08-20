using BondTalesChat_Server.Data;
using BondTalesChat_Server.Hubs;
using BondTalesChat_Server.models;
using BondTalesChat_Server.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Any;

namespace BondTalesChat_Server.Services
{
    public interface IMessageService
    {
        Task<MessageModel> SaveAndBroadcastAsync(int ConversationId, int senderId, string Messagetext, string MediaUrl, byte MessageType, bool Edited, bool Deleted);
         Task<MessageModel[]> GetMessagesOfCurrentLoginUser(int loginuserId);

        Task<List<UserModel>> GetAllUsersChatList();

    }

    public class MessageService : IMessageService
    {
        private readonly AppDbContext _db;
        private readonly IHubContext<ChatHub> _hub;
        private readonly string _connectionString;

        public MessageService(AppDbContext context, IHubContext<ChatHub> hubContext,IConfiguration configuration)
        {
            _db = context;
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
                SentAt= DateTime.UtcNow
            };

            _db.Messages.Add(msg);
            await _db.SaveChangesAsync(); // EF automatically sets MessageId after this

            await _hub.Clients.All.SendAsync("ReceiveMessage", msg);
            return msg;
        }

        public async Task<MessageModel[]> GetMessagesOfCurrentLoginUser(int loginuserId)
        {
            return await _db.Messages.Where(m => m.SenderId == loginuserId).
                OrderBy(m => m.SentAt)
                .ToArrayAsync();
        }


        public async Task<List<UserModel>> GetAllUsersChatList()
        {
            var users = new List<UserModel>();

            await using (var con = new SqlConnection(_connectionString))
            {
                await con.OpenAsync();

                var query = "SELECT UserId, username, email, userpassword, ProfilePicture, CreatedAt, phoneNumber FROM Users";

                using (var cmd = new SqlCommand(query, con))
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
