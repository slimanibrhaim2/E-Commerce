using WebApi.Authentication.DTOs;

namespace WebApi.Authentication.Services
{
    public interface IOtpService
    {
        Task<bool> SendOtpAsync(string phoneNumber);
        Task<bool> VerifyOtpAsync(string phoneNumber, string otp);
        Task<string> GenerateJwtTokenAsync(Guid userId,string phoneNumber);
    }
} 