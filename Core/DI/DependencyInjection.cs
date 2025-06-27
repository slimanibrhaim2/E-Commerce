using Core.Interfaces;
using Core.Services;
using Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Core.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register FileStorageSettings
            var fileStorageSettings = configuration.GetSection("FileStorage").Get<FileStorageSettings>();
            services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));
            
            // Register FileService
            services.AddScoped<IFileService, FileService>();
            
            return services;
        }
    }
} 