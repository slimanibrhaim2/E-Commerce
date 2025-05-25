using Catalogs.Presentation.Controllers;
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
using Catalogs.Application.Commands.CreateProduct;
using Catalogs.Application.Commands.CreateProduct.Simple;

namespace Catalogs.Presentation.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCatalogPresentation(this IServiceCollection services)
        {
            services.AddControllers()
                    .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(ProductsController).Assembly));

            // Add MediatR
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly);
            });

            return services;
        }
    }
}
