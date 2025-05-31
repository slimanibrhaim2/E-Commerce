using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
// using FluentValidation; // Removed

namespace Shoppings.Application.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddShoppingsApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddMediatR(configuration =>
                configuration.RegisterServicesFromAssembly(assembly));
            // services.AddValidatorsFromAssembly(assembly); // Removed
            return services;
        }
    }
}
