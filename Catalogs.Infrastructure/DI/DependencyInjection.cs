using Catalogs.Domain.Repositories;
using Catalogs.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Infrastructure.Common;
using Catalogs.Infrastructure.Mapping.Mappers;
using Infrastructure.Models;
using Catalogs.Domain.Entities;
using Core.Interfaces;

namespace Catalogs.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCatalogsInfrastructure(this IServiceCollection services)
        {
            // Register Repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IProductFeatureRepository, ProductFeatureRepository>();
            services.AddScoped<IServiceFeatureRepository, ServiceFeatureRepository>();
            services.AddScoped<IMediaRepository, MediaRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBaseItemRepository, BaseItemRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            services.AddScoped<IMediaTypeRepository, MediaTypeRepository>();
            services.AddScoped<IFeatureRepository, FeatureRepository>();

            // Register Mappers
            services.AddScoped<IMapper<ProductDAO, Product>, ProductMapper>();
            services.AddScoped<IMapper<ServiceDAO, Service>, ServiceMapper>();
            services.AddScoped<IMapper<CategoryDAO, Category>, CategoryMapper>();
            services.AddScoped<IMapper<ProductFeatureDAO, ProductFeature>, ProductFeatureMapper>();
            services.AddScoped<IMapper<ServiceFeatureDAO, ServiceFeature>, ServiceFeatureMapper>();
            services.AddScoped<IMapper<MediaDAO, Media>, MediaMapper>();
            services.AddScoped<IMapper<BaseItemDAO, BaseItem>, BaseItemMapper>();
            services.AddScoped<IMapper<FavoriteDAO, Favorite>, FavoriteMapper>();
            services.AddScoped<IMapper<MediaTypeDAO, MediaType>, MediaTypeMapper>();

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
} 