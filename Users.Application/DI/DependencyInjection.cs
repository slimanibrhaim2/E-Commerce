using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Repositories;

namespace Users.Application.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUsersApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(assembly));
            return services;
        }
    }

}
