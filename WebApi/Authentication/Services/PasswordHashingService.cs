using System.Security.Cryptography;
using System.Text;

namespace WebApi.Authentication.Services
{
    public class PasswordHashingService
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 10000;

        public static string HashPassword(string password)
        {
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

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                // Decode the base64 string
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);

                // Extract salt (first 16 bytes)
                byte[] salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                // Extract hash (remaining bytes)
                byte[] hash = new byte[HashSize];
                Array.Copy(hashBytes, SaltSize, hash, 0, HashSize);

                // Hash the provided password with the extracted salt
                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
                byte[] testHash = pbkdf2.GetBytes(HashSize);

                // Compare hashes
                return hash.SequenceEqual(testHash);
            }
            catch
            {
                return false;
            }
        }
    }
}