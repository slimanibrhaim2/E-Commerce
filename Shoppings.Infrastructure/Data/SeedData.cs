using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Shoppings.Domain.Repositories;
using Core.Interfaces;
using Shoppings.Domain.Entities;

namespace Shoppings.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task SeedShoppingData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var orderStatusRepo = scope.ServiceProvider.GetRequiredService<IOrderStatusRepository>();

            // Seed Order Statuses
            if (!(await orderStatusRepo.GetAllAsync()).Any())
            {
                var orderStatuses = new[]
                {
                    new OrderStatus
                    {
                        Id = Guid.NewGuid(),
                        Name = "قيد الانتظار",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new OrderStatus
                    {
                        Id = Guid.NewGuid(),
                        Name = "تم التأكيد",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new OrderStatus
                    {
                        Id = Guid.NewGuid(),
                        Name = "قيد التحضير",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new OrderStatus
                    {
                        Id = Guid.NewGuid(),
                        Name = "قيد الشحن",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new OrderStatus
                    {
                        Id = Guid.NewGuid(),
                        Name = "تم التوصيل",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new OrderStatus
                    {
                        Id = Guid.NewGuid(),
                        Name = "ملغي",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new OrderStatus
                    {
                        Id = Guid.NewGuid(),
                        Name = "مسترد",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                foreach (var status in orderStatuses)
                {
                    await orderStatusRepo.AddAsync(status);
                }
            }

            await unitOfWork.SaveChangesAsync();
        }
    }
} 