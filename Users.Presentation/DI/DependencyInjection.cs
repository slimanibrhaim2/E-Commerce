using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Application.DTOs;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Users.Presentation.Controllers;

namespace Users.Presentation.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUserPresentation(this IServiceCollection services)
        {
            services.AddControllers()
                    .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(UsersController).Assembly));

            return services;
        }
    }
}
