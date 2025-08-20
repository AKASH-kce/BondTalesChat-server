using System.Security.Cryptography;

namespace BondTalesChat_Server.Services
{
    public class OtpService
    {
        private readonly Dictionary<string, (string Otp, DateTime Expiry)> _otps = new();

        public OtpService() // ← No dependencies
        {
            Console.WriteLine($"[OtpService] Initialized: {this.GetHashCode()}");
        }
        public string GenerateOtp()
        {
            var bytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            return (BitConverter.ToUInt32(bytes, 0) % 100000).ToString("D6");
        }
        private static void LogStringBytes(string label, string text)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            Console.WriteLine($"{label} (length={text.Length}): '{text}' | Bytes: [{string.Join(", ", bytes)}]");
        }
        public void StoreOtp(string email, string otp)
        {
            var key = email.ToLower().Trim();
            LogStringBytes("[OtpService] 📦 Storing OTP for email", key);
            Console.WriteLine($"[OtpService] 🔐 Generated OTP: {otp}"); // ← Add this

            var expiry = DateTime.UtcNow.AddMinutes(10);
            _otps[key] = (otp, expiry);
            Console.WriteLine($"[OtpService] ✅ OTP stored. Total in dict: {_otps.Count}");
        }

        public bool ValidateOtp(string email, string otp)
        {
            var key = email.ToLower().Trim();
            LogStringBytes("[OtpService] 🔍 Validating OTP for email", key);
            Console.WriteLine($"[OtpService] 🔐 Provided OTP: {otp}");
            Console.WriteLine($"[OtpService] 📁 Current keys: [{string.Join(", ", _otps.Keys)}]");

            if (!_otps.TryGetValue(key, out var stored))
            {
                Console.WriteLine("❌ No such key in dictionary.");
                return false;
            }

            Console.WriteLine($"[OtpService] ✅ Found stored OTP: {stored.Otp}, Expires: {stored.Expiry}");

            if (stored.Expiry < DateTime.UtcNow)
            {
                _otps.Remove(key);
                Console.WriteLine("❌ OTP expired");
                return false;
            }

            if (stored.Otp != otp)
            {
                Console.WriteLine("❌ OTP mismatch");
                return false;
            }

            _otps.Remove(key);
            Console.WriteLine("✅ OTP validated and removed");
            return true;
        }
        public bool IsOtpRequested(string email)
        {
            return _otps.ContainsKey(email);
        }
    }
}
