using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;

namespace BondTalesChat_Server.Services
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string recipientEmail, string otp);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendOtpEmailAsync(string recipientEmail, string otp)
        {
            var smtpConfig = _configuration.GetSection("Smtp");
            var fromEmail = smtpConfig["FromEmail"] ?? throw new InvalidOperationException("Smtp:FromEmail is not configured.");
            var fromName = smtpConfig["FromName"] ?? "BondTales Support";
            var host = smtpConfig["Host"] ?? throw new InvalidOperationException("Smtp:Host is not configured.");
            var port = int.Parse(smtpConfig["Port"] ?? "587");
            var username = smtpConfig["Username"] ?? throw new InvalidOperationException("Smtp:Username is not configured.");
            var password = smtpConfig["Password"] ?? throw new InvalidOperationException("Smtp:Password is not configured.");
            var enableSsl = bool.Parse(smtpConfig["EnableSsl"] ?? "true");

            var subject = "Your Password Reset OTP";
            var body = $@"
                <h2>Password Reset OTP</h2>
                <p>Hello,</p>
                <p>We received a request to reset your password.</p>
                <h3>Your OTP: <strong>{otp}</strong></h3>
                <p>This code is valid for 10 minutes. Do not share it with anyone.</p>
                <p>If you didn’t request this, please ignore this email.</p>
                <br>
                <p>Best regards,<br>{fromName}</p>";

            using (var client = new SmtpClient(host, port))
            {
                client.EnableSsl = enableSsl;
                client.Credentials = new NetworkCredential(username, password);

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(fromEmail, fromName);
                    message.To.Add(new MailAddress(recipientEmail));
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    try
                    {
                        await client.SendMailAsync(message);
                        Console.WriteLine($"✅ OTP email sent successfully to {recipientEmail}");
                    }
                    catch (SmtpException smtpEx)
                    {
                        Console.WriteLine($"❌ SMTP Error: {smtpEx.Message}");
                        throw new Exception("Failed to send email. Check your SMTP configuration.", smtpEx);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ General Error: {ex.Message}");
                        throw new Exception("An unexpected error occurred while sending the email.", ex);
                    }
                }
            }
        }
    }
}