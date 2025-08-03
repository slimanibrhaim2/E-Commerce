using Microsoft.Extensions.Options;

namespace WebApi.Authentication.Services
{
    public class TokenExpirationService : ITokenExpirationService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenExpirationService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public DateTime GetExpirationByUserType(string userType)
        {
            return userType switch
            {
                "user" => DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes), // Default: 7 days (10080 minutes)
                "admin" => DateTime.UtcNow.AddDays(30), // Admin: 30 days for better security
                "rating_system" => DateTime.UtcNow.AddYears(1), // Rating System: 1 year for convenience
                _ => DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes) // Default fallback
            };
        }

        public TimeSpan GetTokenLifetime(string userType)
        {
            return userType switch
            {
                "user" => TimeSpan.FromMinutes(_jwtSettings.ExpiryInMinutes),
                "admin" => TimeSpan.FromDays(30),
                "rating_system" => TimeSpan.FromDays(365),
                _ => TimeSpan.FromMinutes(_jwtSettings.ExpiryInMinutes)
            };
        }

        public bool IsLongLivedToken(string userType)
        {
            return userType == "rating_system" || userType == "admin";
        }
    }
}