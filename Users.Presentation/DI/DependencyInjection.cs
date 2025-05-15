using Microsoft.Extensions.DependencyInjection;


namespace Users.Presentation.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUserPresentation(this IServiceCollection services)
        {
            return services;
        }
    }
}
