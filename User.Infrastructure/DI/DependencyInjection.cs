using Core.Interfaces;
using Infrastructure.Common;
using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Domain.Repositories;
using Users.Infrastructure.Repositories;
using Users.Domain.Entities;
using Users.Infrastructure.Mapping.Mappers;

namespace Users.Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddUserInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Register mappers
        services.AddScoped<IMapper<UserDAO, User>, UserMapper>();
        services.AddScoped<IMapper<AddressDAO, Address>, AddressMapper>();
        services.AddScoped<IMapper<FollowerDAO, Follower>, FollowerMapper>();
        
        return services;
    }
}
