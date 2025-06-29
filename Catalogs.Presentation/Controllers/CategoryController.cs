using Microsoft.AspNetCore.Mvc;
using MediatR;
using Catalogs.Application.Queries.GetAllCategories;
using Catalogs.Application.Commands.CreateCategory;
using Catalogs.Application.Commands.UpdateCategory;
using Catalogs.Application.Commands.DeleteCategory;
using Catalogs.Application.DTOs;
using Microsoft.Extensions.Logging;
using Catalogs.Application.Queries.GetCategoryById;
using Catalogs.Application.Queries.GetSubCategories;
using Core.Pagination;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;
using Core.Result;
using Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Web;

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CategoryController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;

        public CategoryController(
            IMediator mediator,
            ILogger<CategoryController> logger,
            IWebHostEnvironment webHostEnvironment,
            IFileService fileService)
        {
            _mediator = mediator;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParameters parameters)
        {
            var query = new GetAllCategoriesQuery(parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "Failed to get categories",
                    errorType: "GetCategoriesFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<PaginatedResult<CategoryDTO>>.Ok(
                data: result.Data,
                message: "Categories retrieved successfully",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetCategoryByIdQuery(id);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "Failed to get category",
                    errorType: "GetCategoryByIdFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<CategoryDTO>.Ok(
                data: result.Data,
                message: "Category retrieved successfully",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("{parentId}/subcategories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSubCategories(Guid parentId, [FromQuery] PaginationParameters parameters)
        {
            var query = new GetSubCategoriesQuery(parentId, parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "Failed to get subcategories",
                    errorType: "GetSubCategoriesFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<PaginatedResult<CategoryDTO>>.Ok(
                data: result.Data,
                message: "Subcategories retrieved successfully",
                resultStatus: ResultStatus.Success));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateCategoryRequest request, IFormFile imageFile)
        {
            try
            {
                string imageUrl = null;
                if (imageFile != null)
                {
                    // Validate file
                    var validationResult = _fileService.ValidateFile(
                        imageFile,
                        new[] { "image/jpeg", "image/png", "image/gif" },
                        5 * 1024 * 1024 // 5MB limit
                    );

                    if (!validationResult.Success)
                    {
                        _logger.LogWarning("File validation failed: {Error}", validationResult.Message);
                        return BadRequest(validationResult);
                    }

                    // Save file using FileService
                    var saveResult = await _fileService.SaveFileAsync(imageFile, "media/categories");
                    if (!saveResult.Success)
                    {
                        _logger.LogError("File save failed: {Error}", saveResult.Message);
                        return StatusCode(500, saveResult);
                    }

                    imageUrl = saveResult.Data;
                }

                // Create the DTO for the command
                var dto = new CreateCategoryDTO
                {
                    Name = request.Name,
                    Description = request.Description,
                    ParentId = request.ParentId,
                    IsActive = request.IsActive,
                    ImageUrl = imageUrl
                };

                var command = new CreateCategoryCommand(dto);
                var result = await _mediator.Send(command);
                
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "Failed to create category",
                        errorType: "CreateCategoryFailed",
                        resultStatus: ResultStatus.Failed));

                return CreatedAtAction(nameof(GetById), new { id = result.Data }, Result<Guid>.Ok(
                    data: result.Data,
                    message: "Category created successfully",
                    resultStatus: ResultStatus.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, Result.Fail(
                    message: $"Failed to create category: {ex.Message}",
                    errorType: "CreateCategoryFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] CreateCategoryRequest request, IFormFile imageFile)
        {
            try
            {
                string imageUrl = null;
                if (imageFile != null)
                {
                    // Validate file
                    var validationResult = _fileService.ValidateFile(
                        imageFile,
                        new[] { "image/jpeg", "image/png", "image/gif" },
                        5 * 1024 * 1024 // 5MB limit
                    );

                    if (!validationResult.Success)
                    {
                        _logger.LogWarning("File validation failed: {Error}", validationResult.Message);
                        return BadRequest(validationResult);
                    }

                    // Get current category to handle image update
                    var currentCategory = await _mediator.Send(new GetCategoryByIdQuery(id));
                    if (currentCategory.Success && !string.IsNullOrEmpty(currentCategory.Data.ImageUrl))
                    {
                        // Delete old image file
                        var deleteResult = _fileService.DeleteFile(currentCategory.Data.ImageUrl);
                        if (!deleteResult.Success)
                        {
                            _logger.LogWarning("Failed to delete old image: {Error}", deleteResult.Message);
                        }
                    }

                    // Save new file using FileService
                    var saveResult = await _fileService.SaveFileAsync(imageFile, "media/categories");
                    if (!saveResult.Success)
                    {
                        _logger.LogError("File save failed: {Error}", saveResult.Message);
                        return StatusCode(500, saveResult);
                    }

                    imageUrl = saveResult.Data;
                }
                else
                {
                    // If no new image is provided, keep the existing image URL
                    var currentCategory = await _mediator.Send(new GetCategoryByIdQuery(id));
                    if (currentCategory.Success)
                    {
                        imageUrl = currentCategory.Data.ImageUrl;
                    }
                }

                // Create the DTO for the command
                var dto = new CreateCategoryDTO
                {
                    Name = request.Name,
                    Description = request.Description,
                    ParentId = request.ParentId,
                    IsActive = request.IsActive,
                    ImageUrl = imageUrl
                };

                var command = new UpdateCategoryCommand(id, dto);
                var result = await _mediator.Send(command);
                
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "Failed to update category",
                        errorType: "UpdateCategoryFailed",
                        resultStatus: ResultStatus.Failed));

                return Ok(Result.Ok(
                    message: "Category updated successfully",
                    resultStatus: ResultStatus.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category");
                return StatusCode(500, Result.Fail(
                    message: $"Failed to update category: {ex.Message}",
                    errorType: "UpdateCategoryFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                // Get category to handle image deletion
                var currentCategory = await _mediator.Send(new GetCategoryByIdQuery(id));
                if (currentCategory.Success && !string.IsNullOrEmpty(currentCategory.Data.ImageUrl))
                {
                    // Delete image file using FileService
                    var deleteResult = _fileService.DeleteFile(currentCategory.Data.ImageUrl);
                    if (!deleteResult.Success)
                    {
                        _logger.LogWarning("Failed to delete image: {Error}", deleteResult.Message);
                    }
                }

                var command = new DeleteCategoryCommand(id);
                var result = await _mediator.Send(command);
                
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                            message: "Failed to delete category",
                            errorType: "DeleteCategoryFailed",
                            resultStatus: ResultStatus.Failed));

                return Ok(Result.Ok(
                    message: "Category deleted successfully",
                    resultStatus: ResultStatus.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                return StatusCode(500, Result.Fail(
                    message: $"Failed to delete category: {ex.Message}",
                    errorType: "DeleteCategoryFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet("image/{*imagePath}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryImage(string imagePath)
        {
            try
            {
                _logger.LogInformation("Original requested image path: {ImagePath}", imagePath);

                // URL decode the file path
                imagePath = HttpUtility.UrlDecode(imagePath);
                _logger.LogInformation("URL decoded image path: {ImagePath}", imagePath);

                // Remove leading slash if present and normalize slashes
                imagePath = imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                _logger.LogInformation("Normalized image path: {ImagePath}", imagePath);

                // Basic path validation to prevent directory traversal
                if (imagePath.Contains(".."))
                {
                    _logger.LogWarning("Invalid image path attempted (directory traversal): {ImagePath}", imagePath);
                    return BadRequest(Result.Fail(
                        message: "Invalid image path",
                        errorType: "InvalidPath",
                        resultStatus: ResultStatus.Failed));
                }

                // Ensure the path starts with media/categories
                if (!imagePath.StartsWith("media" + Path.DirectorySeparatorChar + "categories"))
                {
                    _logger.LogWarning("Invalid category image directory attempted: {ImagePath}", imagePath);
                    return BadRequest(Result.Fail(
                        message: "Invalid category image directory",
                        errorType: "InvalidPath",
                        resultStatus: ResultStatus.Failed));
                }

                // Get the result using IFileService
                var result = await _fileService.GetFileAsync(imagePath);
                if (!result.Success)
                {
                    _logger.LogWarning("Category image not found: {ImagePath}", imagePath);
                    return NotFound(Result.Fail(
                        message: "Category image not found",
                        errorType: "FileNotFound",
                        resultStatus: ResultStatus.Failed));
                }

                // Determine content type based on file extension
                var extension = Path.GetExtension(imagePath).ToLowerInvariant();
                var contentType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    _ => "application/octet-stream"
                };

                _logger.LogInformation("Serving category image with content type: {ContentType}", contentType);

                // Set cache control headers for better performance
                Response.Headers.Add("Cache-Control", "public, max-age=31536000"); // Cache for 1 year
                
                // Return the file stream
                return File(result.Data, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category image: {ImagePath}", imagePath);
                return StatusCode(500, Result.Fail(
                    message: "Error retrieving category image",
                    errorType: "FileRetrievalError",
                    resultStatus: ResultStatus.Failed));
            }
        }
    }
} 