using Core.Interfaces;
using Infrastructure.Common;
using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork,UnitOfWork>();
            services.AddDbContext<ECommerceContext>();
            return services;
        }
     }
   
}
