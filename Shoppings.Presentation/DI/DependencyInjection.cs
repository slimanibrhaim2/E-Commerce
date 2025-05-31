using Shoppings.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System.Reflection;
using Shoppings.Application.Commands.CreateOrder;

namespace Shoppings.Presentation.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddShoppingsPresentation(this IServiceCollection services)
        {
            services.AddControllers()
                    .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(CartController).Assembly));

            // Add MediatR
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly);
            });

            return services;
        }
    }
}