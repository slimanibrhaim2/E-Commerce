using Catalogs.Domain.Entities;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Catalogs.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task SeedMediaTypes(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ECommerceContext>();

            // Check if media types already exist
            if (await context.MediaTypes.AnyAsync())
            {
                return;
            }

            var mediaTypes = new List<MediaTypeDAO>
            {
                new MediaTypeDAO
                {
                    Id = Guid.NewGuid(),
                    Name = "Image",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new MediaTypeDAO
                {
                    Id = Guid.NewGuid(),
                    Name = "Video",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new MediaTypeDAO
                {
                    Id = Guid.NewGuid(),
                    Name = "Document",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new MediaTypeDAO
                {
                    Id = Guid.NewGuid(),
                    Name = "Audio",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.MediaTypes.AddRangeAsync(mediaTypes);
            await context.SaveChangesAsync();
        }
    }
} 