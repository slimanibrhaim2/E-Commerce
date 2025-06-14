using WebApi.Authentication.DTOs;
using Microsoft.Extensions.Options;
using WebApi.Authentication;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
namespace WebApi.Authentication.Services
{
    public class OtpService : IOtpService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<OtpService> _logger;
        private static readonly Dictionary<string, (string Otp, DateTime Expiry)> _otpStore = new();

        public OtpService(IOptions<JwtSettings> jwtSettings, ILogger<OtpService> logger)
        {
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<bool> SendOtpAsync(string phoneNumber)
        {
            try
            {
                // Always use "1111" as OTP for testing
                const string otp = "1111";
                var expiry = DateTime.UtcNow.AddMinutes(5);

                // Store OTP with expiry
                _otpStore[phoneNumber] = (otp, expiry);

                _logger.LogInformation("OTP sent for {PhoneNumber}: {Otp}, Expires at: {Expiry}", 
                    phoneNumber, otp, expiry);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP to {PhoneNumber}", phoneNumber);
                return false;
            }
        }

        public async Task<bool> VerifyOtpAsync(string phoneNumber, string otp)
        {
            try
            {
                _logger.LogInformation("Verifying OTP for {PhoneNumber}. Received OTP: {Otp}", 
                    phoneNumber, otp);

                if (!_otpStore.TryGetValue(phoneNumber, out var storedOtp))
                {
                    _logger.LogWarning("No OTP found for {PhoneNumber}", phoneNumber);
                    return false;
                }

                _logger.LogInformation("Stored OTP for {PhoneNumber}: {StoredOtp}, Expires at: {Expiry}", 
                    phoneNumber, storedOtp.Otp, storedOtp.Expiry);

                if (DateTime.UtcNow > storedOtp.Expiry)
                {
                    _logger.LogWarning("OTP expired for {PhoneNumber}", phoneNumber);
                    _otpStore.Remove(phoneNumber);
                    return false;
                }

                if (storedOtp.Otp != otp)
                {
                    _logger.LogWarning("Invalid OTP for {PhoneNumber}. Expected: {Expected}, Received: {Received}", 
                        phoneNumber, storedOtp.Otp, otp);
                    return false;
                }

                // Remove OTP after successful verification
                _otpStore.Remove(phoneNumber);
                _logger.LogInformation("OTP verified successfully for {PhoneNumber}", phoneNumber);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for {PhoneNumber}", phoneNumber);
                return false;
            }
        }

        public async Task<string> GenerateJwtTokenAsync(Guid userId,string phoneNumber)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.MobilePhone, phoneNumber)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
} 