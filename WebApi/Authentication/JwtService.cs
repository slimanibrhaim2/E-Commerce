using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi.Authentication
{
    public class JwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(Guid userId, string userType = "user")
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // This is the ID claim
                new Claim(ClaimTypes.Role, userType), // User type claim
            };

            // Determine expiration based on user type
            var expiration = GetExpirationByUserType(userType);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private DateTime GetExpirationByUserType(string userType)
        {
            return userType switch
            {
                "user" => DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes), // Default: 7 days
                "admin" => DateTime.UtcNow.AddDays(30), // Admin: 30 days
                "rating_system" => DateTime.UtcNow.AddYears(1), // Rating System: 1 year
                _ => DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes) // Default fallback
            };
        }
    }
}
