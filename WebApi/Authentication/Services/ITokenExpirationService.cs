namespace WebApi.Authentication.Services
{
    public interface ITokenExpirationService
    {
        DateTime GetExpirationByUserType(string userType);
        TimeSpan GetTokenLifetime(string userType);
        bool IsLongLivedToken(string userType);
    }
}