using System.Collections.Concurrent;

namespace BondTalesChat_Server.Services
{
    public class CallSession
    {
        public string CallId { get; set; } = string.Empty;
        public int CallerUserId { get; set; }
        public int CalleeUserId { get; set; }
        public string CallType { get; set; } = "audio"; // 'audio' | 'video'
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }

    public interface ICallStateService
    {
        CallSession Create(string callId, int callerId, int calleeId, string callType);
        CallSession? GetById(string callId);
        int? GetOtherUserId(string callId, int currentUserId);
        void End(string callId);
    }

    public class CallStateService : ICallStateService
    {
        private readonly ConcurrentDictionary<string, CallSession> _sessions = new();

        public CallSession Create(string callId, int callerId, int calleeId, string callType)
        {
            var session = new CallSession
            {
                CallId = callId,
                CallerUserId = callerId,
                CalleeUserId = calleeId,
                CallType = callType
            };
            _sessions[callId] = session;
            return session;
        }

        public CallSession? GetById(string callId)
        {
            _sessions.TryGetValue(callId, out var session);
            return session;
        }

        public int? GetOtherUserId(string callId, int currentUserId)
        {
            var s = GetById(callId);
            if (s == null) return null;
            if (s.CallerUserId == currentUserId) return s.CalleeUserId;
            if (s.CalleeUserId == currentUserId) return s.CallerUserId;
            return null;
        }

        public void End(string callId)
        {
            if (_sessions.TryGetValue(callId, out var s))
            {
                s.IsActive = false;
                _sessions.TryRemove(callId, out _);
            }
        }
    }
}


