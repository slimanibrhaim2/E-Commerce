using Core.Interfaces;
using Core.Result;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Core.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public FileService(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        public async Task<Result<string>> SaveFileAsync(IFormFile file, string folderPath)
        {
            try
            {
                // Create folder if it doesn't exist
                var uploadPath = Path.Combine(_environment.WebRootPath, folderPath);
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Generate unique filename with date folder structure
                var dateFolder = DateTime.UtcNow.ToString("yyyy/MM/dd");
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var relativePath = Path.Combine(folderPath, dateFolder, fileName);
                var fullPath = Path.Combine(_environment.WebRootPath, relativePath);

                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

                // Save file
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return URL
                var baseUrl = _configuration["BaseUrl"] ?? "";
                var fileUrl = $"{baseUrl}/{relativePath.Replace("\\", "/")}";

                return Result<string>.Ok(
                    data: fileUrl,
                    message: "File uploaded successfully",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                return Result<string>.Fail(
                    message: $"Failed to save file: {ex.Message}",
                    errorType: "SaveFileFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }

        public Result<bool> ValidateFile(IFormFile file, string[] allowedTypes, long maxSize)
        {
            try
            {
                // Check if file exists
                if (file == null || file.Length == 0)
                    return Result<bool>.Fail(
                        message: "No file was uploaded",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);

                // Check file size
                if (file.Length > maxSize)
                    return Result<bool>.Fail(
                        message: $"File size exceeds maximum allowed size of {maxSize / 1024 / 1024}MB",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);

                // Check file type
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                    return Result<bool>.Fail(
                        message: $"File type not allowed. Allowed types: {string.Join(", ", allowedTypes)}",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);

                return Result<bool>.Ok(
                    data: true,
                    message: "File validation successful",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(
                    message: $"File validation failed: {ex.Message}",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }

        public Result<bool> DeleteFile(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
                if (!File.Exists(fullPath))
                    return Result<bool>.Fail(
                        message: "File not found",
                        errorType: "NotFound",
                        resultStatus: ResultStatus.NotFound);

                File.Delete(fullPath);
                return Result<bool>.Ok(
                    data: true,
                    message: "File deleted successfully",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(
                    message: $"Failed to delete file: {ex.Message}",
                    errorType: "DeleteFileFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }

        public async Task<Result<Stream>> GetFileAsync(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
                if (!File.Exists(fullPath))
                    return Result<Stream>.Fail(
                        message: "File not found",
                        errorType: "NotFound",
                        resultStatus: ResultStatus.NotFound);

                var memory = new MemoryStream();
                using (var stream = new FileStream(fullPath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                return Result<Stream>.Ok(
                    data: memory,
                    message: "File retrieved successfully",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                return Result<Stream>.Fail(
                    message: $"Failed to get file: {ex.Message}",
                    errorType: "GetFileFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 