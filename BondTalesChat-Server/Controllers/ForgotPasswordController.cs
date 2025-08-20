using BondTalesChat_Server.Models.Dto;
using BondTalesChat_Server.Repositories;
using BondTalesChat_Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace BondTalesChat_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserRepository _userRepository;
        private readonly JwtService _jwtService;
        private readonly OtpService _otpService;
        private readonly IEmailService _emailService;

        //public ForgotPasswordController(IConfiguration configuration)
        //{
        //    _userRepository = new UserRepository(configuration);
        //    _jwtService = new JwtService(configuration);
        //    _otpService = new OtpService();
        //    _emailService = new EmailService(configuration);
        //    _configuration = configuration;
        //}

        public ForgotPasswordController(
        IConfiguration configuration,
        JwtService jwtService,
        OtpService otpService,
        IEmailService emailService)
        {
            _configuration = configuration;
            _userRepository = new UserRepository(configuration);
            _jwtService = jwtService;
            _otpService = otpService;
            _emailService = emailService;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotpasswordDto)
        {
            if (string.IsNullOrWhiteSpace(forgotpasswordDto.Email) || string.IsNullOrWhiteSpace(forgotpasswordDto.Username))
            {
                return BadRequest(new { success = false, message = "Email and username are required." });
            }

            var user = _userRepository.GetUserByEmail(forgotpasswordDto.Email);

            if (user == null)
            {
                return BadRequest(new { success = false, message = "create an account." });
            }

            else if (user.username != forgotpasswordDto.Username)
            {
                return BadRequest(new { success = false, message = "Invalid username provided." });
            }

            var otp = _otpService.GenerateOtp();
            _otpService.StoreOtp(forgotpasswordDto.Email, otp);

            await _emailService.SendOtpEmailAsync(forgotpasswordDto.Email, otp);

            return Ok(new { success = true, message = "OTP sent to your email." });
        }

        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromBody] VerifyOtpDto verifyotpDto)
        {
            if (!_otpService.ValidateOtp(verifyotpDto.Email, verifyotpDto.Otp))
            {
                return BadRequest(new {success = false, message = "Invalid or epired OTP"});
            }
            
            return Ok(new { success = true, message = "Otp Verified, you can reset your password." });
        }

        [HttpPut("reset-password")]
        public IActionResult Resetpassword([FromBody] ResetPasswordDto resetpasswordDto)
        {
            if(string.IsNullOrWhiteSpace(resetpasswordDto.Email) || string.IsNullOrWhiteSpace(resetpasswordDto.NewPassword))
            {
                return BadRequest(new { success = false, message = "Email and new password are required fields." });
            }

            var user = _userRepository.GetUserByEmail(resetpasswordDto.Email);

            if (user == null)
            {
                return BadRequest(new { success = false, message = "user not found," });
            }

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(resetpasswordDto.NewPassword);

            bool updated = _userRepository.UpdatePassword(user.UserId, newPasswordHash);

            if(!updated)
            {
                return StatusCode(500, new { success = false, message = "Failed to update password." });
            }

            var updatedUser = _userRepository.GetUserById(user.UserId);
            var newToken = _jwtService.GenerateToken(updatedUser);

            Response.Cookies.Append("token", newToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"]))
            });

            return Ok(new
            {
                success = true,
                message = "Password reset successfully.",
                token = newToken,
                user = new
                {
                    updatedUser.UserId,
                    updatedUser.username,
                    updatedUser.email,
                    updatedUser.phoneNumber,
                    updatedUser.ProfilePicture
                }
            });

        }
    }
}
