using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Communication.Domain.Repositories;
using Core.Interfaces;
using Communication.Domain.Entities;

namespace Communication.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task SeedCommunicationData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var attachmentTypeRepo = scope.ServiceProvider.GetRequiredService<IAttachmentTypeRepository>();

            // Seed Attachment Types
            if (!(await attachmentTypeRepo.GetAllAsync()).Any())
            {
                var attachmentTypes = new[]
                {
                    new AttachmentType
                    {
                        Id = Guid.NewGuid(),
                        Name = "صورة",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new AttachmentType
                    {
                        Id = Guid.NewGuid(),
                        Name = "فيديو",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new AttachmentType
                    {
                        Id = Guid.NewGuid(),
                        Name = "مستند",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new AttachmentType
                    {
                        Id = Guid.NewGuid(),
                        Name = "صوت",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new AttachmentType
                    {
                        Id = Guid.NewGuid(),
                        Name = "ملف مضغوط",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                foreach (var type in attachmentTypes)
                {
                    await attachmentTypeRepo.AddAsync(type);
                }
            }

            await unitOfWork.SaveChangesAsync();
        }
    }
} 