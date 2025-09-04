using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace BondTalesChat_Server.Services
{
    public class NameIdentifierUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}


