using Microsoft.AspNetCore.Http;
using Core.Result;

namespace Core.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        /// Saves a file to the specified folder and returns its URL
        /// </summary>
        Task<Result<string>> SaveFileAsync(IFormFile file, string folderPath);

        /// <summary>
        /// Validates if the file meets the requirements (size, type, etc.)
        /// </summary>
        Result<bool> ValidateFile(IFormFile file, string[] allowedTypes, long maxSize);

        /// <summary>
        /// Deletes a file at the specified path
        /// </summary>
        Result<bool> DeleteFile(string filePath);

        /// <summary>
        /// Gets a file as a stream from the specified path
        /// </summary>
        Task<Result<Stream>> GetFileAsync(string filePath);
    }
} 