using System.Security.Claims;
using BondTalesChat_Server.Models.Dto;
using BondTalesChat_Server.Services;
using Microsoft.AspNetCore.SignalR;

namespace BondTalesChat_Server.Hubs
{
    public class CallHub : Hub
    {
        private readonly ICallStateService _callStateService;

        public CallHub(ICallStateService callStateService)
        {
            _callStateService = callStateService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
            }
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            Console.WriteLine($"[CallHub] Connected: connectionId={Context.ConnectionId}, userId={userId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(userId))
            {
                userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
            }
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }
            Console.WriteLine($"[CallHub] Disconnected: connectionId={Context.ConnectionId}, userId={userId}");
            await base.OnDisconnectedAsync(exception);
        }

        public Task<string?> WhoAmI()
        {
            var uid = Context.UserIdentifier;
            if (string.IsNullOrEmpty(uid))
            {
                uid = Context.GetHttpContext()?.Request.Query["userId"].ToString();
            }
            return Task.FromResult(uid);
        }

        private int GetCurrentUserId()
        {
            var idStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idStr))
            {
                idStr = Context.GetHttpContext()?.Request.Query["userId"].ToString();
            }
            if (string.IsNullOrEmpty(idStr)) throw new HubException("Unauthorized or userId query missing");
            return int.Parse(idStr);
        }

        public async Task<string> InitiateCall(int participantId, string callType)
        {
            var callerId = GetCurrentUserId();
            var callId = Guid.NewGuid().ToString("N");

            _callStateService.Create(callId, callerId, participantId, callType);

            Console.WriteLine($"[CallHub] InitiateCall: callId={callId}, from={callerId} -> to={participantId}, type={callType}");
            await Clients.Group(participantId.ToString()).SendAsync("IncomingCall", new
            {
                callId,
                participantId = callerId,
                participantName = Context.User?.Identity?.Name ?? "Unknown",
                callType,
                isIncoming = true
            });

            return callId;
        }

        public async Task AnswerCall(string callId, OfferAnswerDto answer)
        {
            var answererId = GetCurrentUserId();
            var callerId = _callStateService.GetOtherUserId(callId, answererId);
            if (callerId == null) throw new HubException("Call not found");

            await Clients.Group(callerId.Value.ToString()).SendAsync("CallAccepted", new { callId, participantId = answererId });
            await Clients.Group(callerId.Value.ToString()).SendAsync("CallAnswer", new { fromUserId = answererId, answer });
        }

        public async Task DeclineCall(string callId)
        {
            var declinerId = GetCurrentUserId();
            var otherId = _callStateService.GetOtherUserId(callId, declinerId);
            if (otherId == null) return;
            await Clients.Group(otherId.Value.ToString()).SendAsync("CallDeclined", new { callId, participantId = declinerId });
            _callStateService.End(callId);
        }

        public async Task EndCall(string callId)
        {
            var userId = GetCurrentUserId();
            var otherId = _callStateService.GetOtherUserId(callId, userId);
            if (otherId == null) return;

            await Clients.Groups(new[] { userId.ToString(), otherId.Value.ToString() })
                .SendAsync("CallEnded", new { callId, participantId = userId });
            _callStateService.End(callId);
        }

        public async Task SendCallOffer(int participantId, OfferAnswerDto offer)
        {
            var fromId = GetCurrentUserId();
            Console.WriteLine($"[CallHub] SendCallOffer: from={fromId} -> to={participantId}");
            await Clients.Group(participantId.ToString()).SendAsync("CallOffer", new { fromUserId = fromId, offer });
        }

        public async Task SendCallAnswer(int participantId, OfferAnswerDto answer)
        {
            var fromId = GetCurrentUserId();
            Console.WriteLine($"[CallHub] SendCallAnswer: from={fromId} -> to={participantId}");
            await Clients.Group(participantId.ToString()).SendAsync("CallAnswer", new { fromUserId = fromId, answer });
        }

        public async Task SendCallCandidate(int participantId, IceCandidateDto candidate)
        {
            var fromId = GetCurrentUserId();
            Console.WriteLine($"[CallHub] SendCallCandidate: from={fromId} -> to={participantId}");
            await Clients.Group(participantId.ToString()).SendAsync("CallCandidate", new { fromUserId = fromId, candidate });
        }

        // Debug helper to validate delivery to a specific user group
        public async Task DebugNotify(int userId, string message)
        {
            var fromId = GetCurrentUserId();
            Console.WriteLine($"[CallHub] DebugNotify: from={fromId} -> to={userId}, message={message}");
            await Clients.Group(userId.ToString()).SendAsync("Debug", new { fromUserId = fromId, message });
        }
    }
}


