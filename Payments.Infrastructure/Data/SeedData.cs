using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Payments.Domain.Repositories;
using Core.Interfaces;
using Payments.Domain.Entities;

namespace Payments.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task SeedPaymentData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var paymentStatusRepo = scope.ServiceProvider.GetRequiredService<IPaymentStatusRepository>();
            var paymentMethodRepo = scope.ServiceProvider.GetRequiredService<IPaymentMethodRepository>();

            // Seed Payment Statuses
            if (!(await paymentStatusRepo.GetAllAsync()).Any())
            {
                var paymentStatuses = new[]
                {
                    new PaymentStatus
                    {
                        Id = Guid.NewGuid(),
                        Name = "قيد الانتظار",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PaymentStatus
                    {
                        Id = Guid.NewGuid(),
                        Name = "مكتمل",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PaymentStatus
                    {
                        Id = Guid.NewGuid(),
                        Name = "فشل",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PaymentStatus
                    {
                        Id = Guid.NewGuid(),
                        Name = "مسترد",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PaymentStatus
                    {
                        Id = Guid.NewGuid(),
                        Name = "ملغي",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                foreach (var status in paymentStatuses)
                {
                    await paymentStatusRepo.AddAsync(status);
                }
            }

            // Seed Payment Methods
            if (!(await paymentMethodRepo.GetAllAsync()).Any())
            {
                var paymentMethods = new[]
                {
                    new PaymentMethod
                    {
                        Id = Guid.NewGuid(),
                        Name = "بطاقة ائتمان",
                        Description = "الدفع باستخدام بطاقة الائتمان",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PaymentMethod
                    {
                        Id = Guid.NewGuid(),
                        Name = "بطاقة خصم",
                        Description = "الدفع باستخدام بطاقة الخصم",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PaymentMethod
                    {
                        Id = Guid.NewGuid(),
                        Name = "باي بال",
                        Description = "الدفع باستخدام باي بال",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PaymentMethod
                    {
                        Id = Guid.NewGuid(),
                        Name = "تحويل بنكي",
                        Description = "الدفع عن طريق التحويل البنكي",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new PaymentMethod
                    {
                        Id = Guid.NewGuid(),
                        Name = "الدفع عند الاستلام",
                        Description = "الدفع عند استلام الطلب",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                foreach (var method in paymentMethods)
                {
                    await paymentMethodRepo.AddAsync(method);
                }
            }

            await unitOfWork.SaveChangesAsync();
        }
    }
} 