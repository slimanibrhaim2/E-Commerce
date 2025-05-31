using Core.Interfaces;
using Infrastructure.Common;
using Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Shoppings.Infrastructure.Mapping.Mappers;
using Shoppings.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppings.Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddShoppingsInfrastructure(this IServiceCollection services)
        {
            //Register Repositories
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartItemRepository, CartItemRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();
            services.AddScoped<IOrderActivityRepository, OrderActivityRepository>();

            // Register mappers
            services.AddScoped<IMapper<CartDAO, Cart>, CartMapper>();
            services.AddScoped<IMapper<CartItemDAO, CartItem>, CartItemMapper>();
            services.AddScoped<IMapper<OrderDAO, Order>, OrderMapper>();
            services.AddScoped<IMapper<OrderItemDAO, OrderItem>, OrderItemMapper>();
            services.AddScoped<IMapper<OrderStatusDAO, OrderStatus>, OrderStatusMapper>();
            services.AddScoped<IMapper<OrderActivityDAO, OrderActivity>, OrderActivityMapper>();

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
