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
using Catalogs.Presentation.ModelBinders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Catalogs.Application.DTOs;

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

        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new FormCollectionModelBinderProvider());
            });

            return services;
        }
    }

    public class FormCollectionModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(List<CreateFeatureDTO>))
            {
                return new FormCollectionModelBinder();
            }

            return null;
        }
    }
}
