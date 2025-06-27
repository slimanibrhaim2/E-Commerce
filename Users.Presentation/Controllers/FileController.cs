using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Interfaces;
using Core.Result;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mime;
using Microsoft.AspNetCore.Hosting;
using System.Web;

namespace Users.Presentation.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FileController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileController(
            IFileService fileService, 
            ILogger<FileController> logger,
            IWebHostEnvironment webHostEnvironment)
        {
            _fileService = fileService;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("{*filePath}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFile(string filePath)
        {
            try
            {
                _logger.LogInformation("Original requested file path: {FilePath}", filePath);

                // URL decode the file path
                filePath = HttpUtility.UrlDecode(filePath);
                _logger.LogInformation("URL decoded file path: {FilePath}", filePath);

                // Remove leading slash if present and normalize slashes
                filePath = filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                _logger.LogInformation("Normalized file path: {FilePath}", filePath);

                // Basic path validation to prevent directory traversal
                if (filePath.Contains(".."))
                {
                    _logger.LogWarning("Invalid file path attempted (directory traversal): {FilePath}", filePath);
                    return BadRequest(Result.Fail(
                        message: "Invalid file path",
                        errorType: "InvalidPath",
                        resultStatus: ResultStatus.Failed));
                }

                // Get the result using IFileService
                var result = await _fileService.GetFileAsync(filePath);
                if (!result.Success)
                {
                    _logger.LogWarning("File not found: {FilePath}", filePath);
                    
                    // Log the full physical path for debugging
                    var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath);
                    _logger.LogWarning("Full physical path that was checked: {FullPath}", fullPath);
                    
                    // Try to list files in the parent directory to help debug
                    try
                    {
                        var directory = Path.GetDirectoryName(fullPath);
                        if (Directory.Exists(directory))
                        {
                            var files = Directory.GetFiles(directory);
                            _logger.LogInformation("Files in directory {Directory}:", directory);
                            foreach (var file in files)
                            {
                                _logger.LogInformation("Found file: {File}", file);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Directory does not exist: {Directory}", directory);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error listing directory contents");
                    }

                    return NotFound(Result.Fail(
                        message: "File not found",
                        errorType: "FileNotFound",
                        resultStatus: ResultStatus.Failed));
                }

                // Determine content type based on file extension
                var extension = Path.GetExtension(filePath).ToLowerInvariant();
                var contentType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".pdf" => "application/pdf",
                    ".doc" or ".docx" => "application/msword",
                    ".xls" or ".xlsx" => "application/vnd.ms-excel",
                    _ => "application/octet-stream"
                };

                _logger.LogInformation("Serving file with content type: {ContentType}", contentType);

                // Set cache control headers for better performance
                Response.Headers.Add("Cache-Control", "public, max-age=31536000"); // Cache for 1 year
                
                // Return the file stream
                return File(result.Data, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving file: {FilePath}", filePath);
                return StatusCode(500, Result.Fail(
                    message: "Error retrieving file",
                    errorType: "FileRetrievalError",
                    resultStatus: ResultStatus.Failed));
            }
        }
    }
} 