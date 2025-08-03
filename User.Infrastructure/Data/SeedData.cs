using Users.Domain.Entities;
using Users.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using UserEntity = Users.Domain.Entities.User;

namespace Users.Infrastructure.Data
{
    public static class SeedData
    {
        // Local password hashing method (same as PasswordHashingService)
        private static string HashPassword(string password)
        {
            const int SaltSize = 16;
            const int HashSize = 32;
            const int Iterations = 10000;

            // Generate a random salt
            byte[] salt = new byte[SaltSize];
            RandomNumberGenerator.Fill(salt);

            // Hash the password with the salt
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Combine salt and hash
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convert to base64 string
            return Convert.ToBase64String(hashBytes);
        }

        public static async Task SeedUserData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("UserSeedData");

            try
            {
                // Check if admin user already exists
                var existingAdmin = await userRepository.GetByEmail("admin@gmail.com");
                if (existingAdmin == null)
                {
                    // Create admin user
                    var adminUser = new UserEntity
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "admin",
                        MiddleName = null,
                        LastName = "admin",
                        PhoneNumber = "0999999999",
                        Email = "admin@gmail.com",
                        Password = HashPassword("adminadmin"),
                        UserType = "admin",
                        Description = "System Administrator",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await userRepository.AddAsync(adminUser);
                    logger.LogInformation("Admin user seeded successfully");
                }

                // Check if rating system user already exists
                var existingRatingSystem = await userRepository.GetByEmail("ratingsys@gmail.com");
                if (existingRatingSystem == null)
                {
                    // Create rating system user
                    var ratingSystemUser = new UserEntity
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "rating",
                        MiddleName = null,
                        LastName = "system",
                        PhoneNumber = "0988888888",
                        Email = "ratingsys@gmail.com",
                        Password = HashPassword("raterate"),
                        UserType = "rating_system",
                        Description = "Rating System Service Account",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await userRepository.AddAsync(ratingSystemUser);
                    logger.LogInformation("Rating system user seeded successfully");
                }

                await unitOfWork.SaveChangesAsync();
                logger.LogInformation("User seed data completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while seeding user data");
                throw;
            }
        }
    }
}