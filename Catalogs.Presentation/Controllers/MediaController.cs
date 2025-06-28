using Microsoft.AspNetCore.Mvc;
using MediatR;
using Catalogs.Application.Commands.AddMedia;
using Catalogs.Application.Commands.DeleteMedia;
using Catalogs.Application.DTOs;
using Microsoft.Extensions.Logging;
using Catalogs.Application.Queries.GetMediaById;
using Catalogs.Application.Commands.UpdateMedia;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;
using Core.Result;
using Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System.Web;

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MediaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MediaController> _logger;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MediaController(
            IMediator mediator, 
            ILogger<MediaController> logger,
            IFileService fileService,
            IWebHostEnvironment webHostEnvironment)
        {
            _mediator = mediator;
            _logger = logger;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("file/{*filePath}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMediaFile(string filePath)
        {
            try
            {
                _logger.LogInformation("Original requested media file path: {FilePath}", filePath);

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

                // Ensure the path starts with media/products or media/services
                if (!filePath.StartsWith("media" + Path.DirectorySeparatorChar + "products") && 
                    !filePath.StartsWith("media" + Path.DirectorySeparatorChar + "services"))
                {
                    _logger.LogWarning("Invalid media directory attempted: {FilePath}", filePath);
                    return BadRequest(Result.Fail(
                        message: "Invalid media directory",
                        errorType: "InvalidPath",
                        resultStatus: ResultStatus.Failed));
                }

                // Get the result using IFileService
                var result = await _fileService.GetFileAsync(filePath);
                if (!result.Success)
                {
                    _logger.LogWarning("Media file not found: {FilePath}", filePath);
                    
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
                        message: "Media file not found",
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
                    ".mp4" => "video/mp4",
                    ".mpeg" => "video/mpeg",
                    _ => "application/octet-stream"
                };

                _logger.LogInformation("Serving media file with content type: {ContentType}", contentType);

                // Set cache control headers for better performance
                Response.Headers.Add("Cache-Control", "public, max-age=31536000"); // Cache for 1 year
                
                // Return the file stream
                return File(result.Data, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving media file: {FilePath}", filePath);
                return StatusCode(500, Result.Fail(
                    message: "Error retrieving media file",
                    errorType: "FileRetrievalError",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPost("{itemId}")]
        public async Task<ActionResult<Result<Guid>>> AddMedia(Guid itemId, [FromBody] CreateMediaDTO dto)
        {
            var command = new AddMediaCommand(itemId, dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "Failed to add media",
                    errorType: "AddMediaFailed",
                    resultStatus: ResultStatus.Failed));

            return CreatedAtAction(nameof(GetById), new { id = result.Data }, Result<Guid>.Ok(
                data: result.Data,
                message: "Media added successfully",
                resultStatus: ResultStatus.Success));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, [FromBody] CreateMediaDTO dto)
        {
            var command = new UpdateMediaCommand(id, dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "Failed to update media",
                    errorType: "UpdateMediaFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "Media updated successfully",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteMediaCommand(id);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "Failed to delete media",
                    errorType: "DeleteMediaFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "Media deleted successfully",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Result<MediaDTO>>> GetById(Guid id)
        {
            var query = new GetMediaByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "Failed to get media",
                    errorType: "GetMediaFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result<MediaDTO>.Ok(
                data: result.Data,
                message: "Media retrieved successfully",
                resultStatus: ResultStatus.Success));
        }
    }
} 