using Microsoft.AspNetCore.Mvc;
using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Application.Commands.CreateProduct.Simple;
using Catalogs.Application.Commands.CreateProduct.Aggregate;
using Catalogs.Application.Queries.GetAllProducts;
using Catalogs.Application.Queries.GetProductById;
using Core.Pagination;
using Catalogs.Application.Commands.DeleteProduct.Aggregate;
using Catalogs.Application.Commands.DeleteProduct.Simple;
using Catalogs.Application.Commands.UpdateProduct.Aggregate;
using Catalogs.Application.Commands.UpdateProduct.Simple;
using Shared.Contracts.DTOs;
using Catalogs.Application.Queries.GetProductsByUserId;
using Catalogs.Application.Queries.GetProductsByCategory;
using Catalogs.Application.Queries.GetProductsByName;
using Catalogs.Application.Queries.GetProductsByIds;
using Catalogs.Application.Queries.GetProductsByPriceRange;
using Catalogs.Application.Queries.GetLowStockProducts;
using Catalogs.Application.Queries.GetBaseItemIdByProductId;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;
using Shared.Contracts.Queries;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Catalogs.Application.Queries.GetAllMediaTypes;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace Catalogs.Presentation.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileService _fileService;
    private readonly ILogger<ProductsController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductsController(IMediator mediator, IFileService fileService, ILogger<ProductsController> logger, IWebHostEnvironment webHostEnvironment)
    {
        _mediator = mediator;
        _fileService = fileService;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDTO dto)
    {
        var userId = User.GetId();
        var result = await _mediator.Send(new CreateProductCommand(dto, userId));
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في إنشاء المنتج",
                errorType: "CreateProductFailed",
                resultStatus: ResultStatus.Failed));
        return CreatedAtAction("GetById", new { id = result.Data }, Result<Guid>.Ok(
            data: result.Data,
            message: "تم إنشاء المنتج بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpPost("aggregate")]
    public async Task<IActionResult> CreateAggregate([FromForm] CreateProductAggregateDTO dto, List<IFormFile> mediaFiles)
    {
        try
        {
            var userId = User.GetId();

            // Parse features from form data
            var featuresJson = Request.Form["features"].ToString();
            _logger.LogInformation("Received features JSON: {JsonString}", featuresJson);

            if (!string.IsNullOrEmpty(featuresJson))
            {
                try
                {
                    // Check if the JSON string starts with [ and ends with ]
                    if (!featuresJson.TrimStart().StartsWith("[") || !featuresJson.TrimEnd().EndsWith("]"))
                    {
                        featuresJson = $"[{featuresJson}]";
                        _logger.LogInformation("Modified features JSON with array brackets: {JsonString}", featuresJson);
                    }
                    
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        WriteIndented = true
                    };
                    
                    var features = JsonSerializer.Deserialize<List<CreateFeatureDTO>>(featuresJson, options);
                    _logger.LogInformation("Deserialized features count: {Count}", features?.Count ?? 0);
                    
                    if (features == null)
                    {
                        _logger.LogError("Features deserialized to null");
                        return BadRequest(new { error = "Features could not be deserialized", receivedJson = featuresJson });
                    }

                    // Log each feature
                    foreach (var feature in features)
                    {
                        _logger.LogInformation("Feature: Name = {Name}, Value = {Value}", feature?.Name, feature?.Value);
                    }
                    
                    // Validate features
                    var validationResults = new List<ValidationResult>();
                    foreach (var feature in features)
                    {
                        if (!Validator.TryValidateObject(feature, new ValidationContext(feature), validationResults, true))
                        {
                            _logger.LogError("Validation failed for feature: {Feature}, Errors: {Errors}", 
                                JsonSerializer.Serialize(feature), 
                                JsonSerializer.Serialize(validationResults));
                            return BadRequest(new { error = "Invalid feature", validationErrors = validationResults });
                        }
                    }
                    dto.Features = features;
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "JSON deserialization error. Raw JSON: {JsonString}", featuresJson);
                    return BadRequest(new { 
                        error = "Invalid features format", 
                        details = ex.Message, 
                        receivedJson = featuresJson,
                        path = ex.Path,
                        lineNumber = ex.LineNumber,
                        bytePositionInLine = ex.BytePositionInLine
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "General error processing features. Raw JSON: {JsonString}", featuresJson);
                    return BadRequest(new { error = "Error processing features", details = ex.Message, receivedJson = featuresJson });
                }
            }

            // Handle media files
            if (mediaFiles != null && mediaFiles.Any())
            {
                // Get all media types using mediator
                var mediaTypesResult = await _mediator.Send(new GetAllMediaTypesQuery(new PaginationParameters { PageNumber = 1, PageSize = 100 }));
                if (!mediaTypesResult.Success)
                {
                    _logger.LogError("Failed to get media types: {Error}", mediaTypesResult.Message);
                    return StatusCode(500, mediaTypesResult);
                }

                var mediaTypes = mediaTypesResult.Data.Data;
                var imageType = mediaTypes.FirstOrDefault(mt => mt.Name.ToLower() == "image");
                var videoType = mediaTypes.FirstOrDefault(mt => mt.Name.ToLower() == "video");

                if (imageType == null || videoType == null)
                {
                    return StatusCode(500, Result.Fail("Media types not properly configured", "ConfigurationError", ResultStatus.Failed));
                }

                var mediaList = new List<CreateMediaDTO>();
                foreach (var file in mediaFiles)
                {
                    // Validate file
                    if (file.Length > 10 * 1024 * 1024) // 10MB limit
                    {
                        return BadRequest(Result.Fail($"File {file.FileName} is too large. Maximum size is 10MB", "ValidationError", ResultStatus.ValidationError));
                    }

                    var allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                    var allowedVideoTypes = new[] { "video/mp4", "video/mpeg" };

                    var mediaTypeId = allowedImageTypes.Contains(file.ContentType) ? imageType.Id :
                                    allowedVideoTypes.Contains(file.ContentType) ? videoType.Id :
                                    Guid.Empty;

                    if (mediaTypeId == Guid.Empty)
                    {
                        return BadRequest(Result.Fail($"File {file.FileName} has unsupported type: {file.ContentType}", "ValidationError", ResultStatus.ValidationError));
                    }

                    // Save file
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var mediaDirectory = allowedImageTypes.Contains(file.ContentType) ? "media/products/images" : "media/products/videos";
                    var directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, mediaDirectory);
                    Directory.CreateDirectory(directoryPath);
                    var filePath = Path.Combine(directoryPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var url = $"/{mediaDirectory}/{fileName}";
                    mediaList.Add(new CreateMediaDTO { Url = url, MediaTypeId = mediaTypeId });
                }

                // Set the media list using the new method
                dto.SetMedia(mediaList);
            }

            var result = await _mediator.Send(new CreateProductAggregateCommand(dto, userId));
            if (!result.Success)
                return StatusCode(500, result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product aggregate");
            return StatusCode(500, Result.Fail(ex.Message, "UnexpectedError", ResultStatus.Failed));
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.Identity.IsAuthenticated ? User.GetId() : Guid.Empty;
        var pagination = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
        var query = new GetAllProductsQuery(pagination, userId);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب المنتجات",
                errorType: "GetAllFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<PaginatedResult<ProductDTO>>.Ok(
            data: result.Data,
            message: "تم جلب المنتجات بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = User.Identity.IsAuthenticated ? User.GetId() : Guid.Empty;
        var query = new GetProductByIdQuery(id, userId);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب المنتج",
                errorType: "GetByIdFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<ProductDTO>.Ok(
            data: result.Data,
            message: "تم جلب المنتج بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpPut("aggregate/{id}")]
    public async Task<IActionResult> UpdateAggregate(Guid id, [FromForm] CreateProductAggregateDTO dto, List<IFormFile> mediaFiles)
    {
        try
        {
            var userId = User.GetId();

            // Get current product to handle media update
            var currentProduct = await _mediator.Send(new GetProductByIdQuery(id, userId));
            if (!currentProduct.Success)
            {
                return StatusCode(500, Result.Fail(
                    message: "فشل في العثور على المنتج",
                    errorType: "ProductNotFound",
                    resultStatus: ResultStatus.NotFound));
            }

            // Handle media files
            if (mediaFiles != null && mediaFiles.Any())
            {
                dto.Media = new List<CreateMediaDTO>();
                foreach (var file in mediaFiles)
                {
                    // Validate file
                    var validationResult = _fileService.ValidateFile(
                        file,
                        new[] { "image/jpeg", "image/png", "image/gif", "video/mp4", "video/mpeg" },
                        10 * 1024 * 1024 // 10MB
                    );

                    if (!validationResult.Success)
                    {
                        _logger.LogWarning("File validation failed: {Error}", validationResult.Message);
                        return BadRequest(validationResult);
                    }

                    // Save file
                    var fileResult = await _fileService.SaveFileAsync(file, "media/products");
                    if (!fileResult.Success)
                    {
                        _logger.LogError("File save failed: {Error}", fileResult.Message);
                        return StatusCode(500, fileResult);
                    }

                    // Add media to DTO
                    dto.Media.Add(new CreateMediaDTO
                    {
                        Url = fileResult.Data,
                        MediaTypeId = file.ContentType.StartsWith("image/") 
                            ? new Guid("YOUR-IMAGE-MEDIA-TYPE-ID") // Replace with actual image media type ID
                            : new Guid("YOUR-VIDEO-MEDIA-TYPE-ID")  // Replace with actual video media type ID
                    });
                }
            }

            // Delete old media files
            if (currentProduct.Data.Media != null)
            {
                foreach (var media in currentProduct.Data.Media)
                {
                    var deleteResult = _fileService.DeleteFile(media.Url);
                    if (!deleteResult.Success)
                    {
                        _logger.LogWarning("Failed to delete media file: {Url}, Error: {Error}", 
                            media.Url, deleteResult.Message);
                    }
                }
            }

            var result = await _mediator.Send(new UpdateProductAggregateCommand(id, dto, userId));
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث المنتج مع الوسائط والميزات",
                    errorType: "UpdateProductAggregateFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result.Ok(
                message: "تم تحديث المنتج مع الوسائط والميزات بنجاح",
                resultStatus: ResultStatus.Success));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product aggregate {ProductId}", id);
            return StatusCode(500, Result.Fail(
                message: $"فشل في تحديث المنتج: {ex.Message}",
                errorType: "UpdateProductAggregateFailed",
                resultStatus: ResultStatus.Failed));
        }
    }

    [HttpDelete("aggregate/{id}")]
    public async Task<IActionResult> DeleteAggregate(Guid id)
    {
        try
        {
            // Get current product to delete media files
            var currentProduct = await _mediator.Send(new GetProductByIdQuery(id, User.GetId()));
            if (currentProduct.Success && currentProduct.Data.Media != null)
            {
                foreach (var media in currentProduct.Data.Media)
                {
                    var deleteResult = _fileService.DeleteFile(media.Url);
                    if (!deleteResult.Success)
                    {
                        _logger.LogWarning("Failed to delete media file: {Url}, Error: {Error}", 
                            media.Url, deleteResult.Message);
                    }
                }
            }

            var result = await _mediator.Send(new DeleteProductAggregateCommand(id));
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف المنتج مع الوسائط والميزات",
                    errorType: "DeleteProductAggregateFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result.Ok(
                message: "تم حذف المنتج مع الوسائط والميزات بنجاح",
                resultStatus: ResultStatus.Success));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product aggregate {ProductId}", id);
            return StatusCode(500, Result.Fail(
                message: "فشل في حذف المنتج مع الوسائط والميزات",
                errorType: "DeleteProductAggregateFailed",
                resultStatus: ResultStatus.Failed));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateProductDTO dto)
    {
        var userId = User.GetId();
        var result = await _mediator.Send(new UpdateProductSimpleCommand(id, dto, userId));
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في تحديث المنتج",
                errorType: "UpdateProductFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result.Ok(
            message: "تم تحديث المنتج بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteProductSimpleCommand(id));
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في حذف المنتج",
                errorType: "DeleteProductFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result.Ok(
            message: "تم حذف المنتج بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpPost("by-ids")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIds([FromBody] IEnumerable<Guid> ids)
    {
        if (ids == null || !ids.Any())
            return BadRequest(Result.Fail("قائمة المعرفات مطلوبة.", "BadRequest", ResultStatus.ValidationError));
        var query = new GetProductsByIdsQuery(ids, new PaginationParameters { PageNumber = 1, PageSize = ids.Count() });
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب المنتجات",
                errorType: "GetProductsByIdsFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<PaginatedResult<ProductDetailsDTO>>.Ok(
            data: result.Data,
            message: "تم جلب المنتجات بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet("my-products")]
    public async Task<IActionResult> GetMyProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.GetId();
        var query = new GetProductsByUserIdQuery(userId, new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize });
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب منتجات المستخدم",
                errorType: "GetProductsByUserIdFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<PaginatedResult<ProductDTO>>.Ok(
            data: result.Data,
            message: "تم جلب منتجات المستخدم بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchProducts([FromQuery] string name, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.Identity.IsAuthenticated ? User.GetId() : Guid.Empty;
        var query = new GetProductsByNameQuery(name, userId, new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize });
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في البحث عن المنتجات",
                errorType: "GetProductsByNameFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<PaginatedResult<ProductDTO>>.Ok(
            data: result.Data,
            message: "تم البحث عن المنتجات بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductsByCategory(Guid categoryId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.Identity.IsAuthenticated ? User.GetId() : Guid.Empty;
        var query = new GetProductsByCategoryQuery(categoryId, userId, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب منتجات الفئة",
                errorType: "GetProductsByCategoryFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<PaginatedResult<ProductDTO>>.Ok(
            data: result.Data,
            message: "تم جلب منتجات الفئة بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet("price-range")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.Identity.IsAuthenticated ? User.GetId() : Guid.Empty;
        var query = new GetProductsByPriceRangeQuery(minPrice, maxPrice, userId, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب المنتجات حسب نطاق السعر",
                errorType: "GetProductsByPriceRangeFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<PaginatedResult<ProductDTO>>.Ok(
            data: result.Data,
            message: "تم جلب المنتجات حسب نطاق السعر بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockProducts([FromQuery] int threshold = 10, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetLowStockProductsQuery(threshold, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب المنتجات ذات المخزون المنخفض",
                errorType: "GetLowStockProductsFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<PaginatedResult<ProductDTO>>.Ok(
            data: result.Data,
            message: "تم جلب المنتجات ذات المخزون المنخفض بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet("{productId}/base-item-id")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBaseItemIdByProductId(Guid productId)
    {
        var query = new GetBaseItemIdByProductIdQuery(productId);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب معرف العنصر الأساسي",
                errorType: "GetBaseItemIdByProductIdFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<Guid>.Ok(
            data: result.Data,
            message: "تم جلب معرف العنصر الأساسي بنجاح",
            resultStatus: ResultStatus.Success));
    }
} 