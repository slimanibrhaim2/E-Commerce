namespace WebApi.Authentication
{
    public class TokenExpirationConfiguration
    {
        public Dictionary<string, string> TokenExpirationByUserType { get; set; } = new();
        
        public TimeSpan ParseExpirationString(string expirationStr)
        {
            if (string.IsNullOrWhiteSpace(expirationStr))
                return TimeSpan.FromDays(7); // Default fallback
                
            var unit = expirationStr[^1..].ToLower();
            var valueStr = expirationStr[..^1];
            
            if (!int.TryParse(valueStr, out var value))
                return TimeSpan.FromDays(7); // Default fallback
                
            return unit switch
            {
                "m" => TimeSpan.FromMinutes(value),
                "h" => TimeSpan.FromHours(value),
                "d" => TimeSpan.FromDays(value),
                "w" => TimeSpan.FromDays(value * 7),
                "y" => TimeSpan.FromDays(value * 365),
                _ => TimeSpan.FromDays(7) // Default fallback
            };
        }
    }
}