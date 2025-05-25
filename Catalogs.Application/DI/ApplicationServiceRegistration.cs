using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using AutoMapper;

namespace Catalogs.Application.DI;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Register AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
} 