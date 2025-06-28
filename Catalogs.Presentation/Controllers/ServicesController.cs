using Microsoft.AspNetCore.Mvc;
using MediatR;
using Core.Result;
using Catalogs.Application.Commands.CreateService;
using Catalogs.Application.Queries.GetAllServices;
using Catalogs.Application.Queries.GetServiceById;
using Catalogs.Application.DTOs;
using Core.Pagination;
using Catalogs.Application.Commands.CreateService.Simple;
using Catalogs.Application.Commands.CreateService.Aggregate;
using Catalogs.Application.Commands.DeleteService.Aggregate;
using Catalogs.Application.Commands.DeleteService.Simple;
using Catalogs.Application.Commands.UpdateService.Aggregate;
using Catalogs.Application.Commands.UpdateService.Simple;
using Shared.Contracts.DTOs;
using Catalogs.Application.Queries.GetServicesByUserId;
using Catalogs.Application.Queries.GetServicesByName;
using Catalogs.Application.Queries.GetServicesByCategory;
using Catalogs.Application.Queries.GetServicesByIds;
using Catalogs.Application.Queries.GetServicesByPriceRange;
using Catalogs.Application.Queries.GetServicesByDuration;
using Catalogs.Application.Queries.GetBaseItemIdByServiceId;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;
using Shared.Contracts.Queries;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Catalogs.Application.Queries.GetAllMediaTypes;

namespace Catalogs.Presentation.Controllers;

[ApiController]
[Route("api/services")]
[Authorize]
public class ServicesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileService _fileService;
    private readonly ILogger<ServicesController> _logger;

    public ServicesController(IMediator mediator, IFileService fileService, ILogger<ServicesController> logger)
    {
        _mediator = mediator;
        _fileService = fileService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceDTO dto)
    {
        var userId = User.GetId();
        var result = await _mediator.Send(new CreateServiceCommand(dto, userId));
        if (!result.Success || result.Data == Guid.Empty)
            return StatusCode(500, Result.Fail(
                message: "فشل في إنشاء الخدمة",
                errorType: "CreateServiceFailed",
                resultStatus: ResultStatus.Failed));
        return CreatedAtAction("GetById", new { id = result.Data }, Result<Guid>.Ok(
            data: result.Data,
            message: "تم إنشاء الخدمة بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpPost("aggregate")]
    public async Task<IActionResult> CreateAggregate([FromForm] CreateServiceAggregateDTO dto, List<IFormFile> mediaFiles)
    {
        try
        {
            var userId = User.GetId();

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
                    _logger.LogError("Required media types (Image/Video) not found in database");
                    return StatusCode(500, Result.Fail(
                        message: "Required media types not found",
                        errorType: "MediaTypesNotFound",
                        resultStatus: ResultStatus.Failed));
                }

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
                    var fileResult = await _fileService.SaveFileAsync(file, "media/services");
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
                            ? imageType.Id
                            : videoType.Id
                    });
                }
            }

            var result = await _mediator.Send(new CreateServiceAggregateCommand(dto, userId));
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في إنشاء الخدمة مع الوسائط والميزات",
                    errorType: "CreateServiceAggregateFailed",
                    resultStatus: ResultStatus.Failed));

            return CreatedAtAction("GetById", new { id = result.Data }, Result<Guid>.Ok(
                data: result.Data,
                message: "تم إنشاء الخدمة مع الوسائط والميزات بنجاح",
                resultStatus: ResultStatus.Success));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating service aggregate");
            return StatusCode(500, Result.Fail(
                message: $"فشل في إنشاء الخدمة: {ex.Message}",
                errorType: "CreateServiceAggregateFailed",
                resultStatus: ResultStatus.Failed));
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.Identity.IsAuthenticated ? User.GetId() : Guid.Empty;
        var pagination = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
        var query = new GetAllServicesQuery(pagination, userId);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب الخدمات",
                errorType: "GetAllFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<PaginatedResult<ServiceDTO>>.Ok(
            data: result.Data,
            message: "تم جلب الخدمات بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = User.Identity.IsAuthenticated ? User.GetId() : Guid.Empty;
        var query = new GetServiceByIdQuery(id, userId);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب الخدمة",
                errorType: "GetByIdFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<ServiceDTO>.Ok(
            data: result.Data,
            message: "تم جلب الخدمة بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpPut("aggregate/{id}")]
    public async Task<IActionResult> UpdateAggregate(Guid id, [FromForm] CreateServiceAggregateDTO dto, List<IFormFile> mediaFiles)
    {
        try
        {
            var userId = User.GetId();

            // Get current service to handle media update
            var currentService = await _mediator.Send(new GetServiceByIdQuery(id, userId));
            if (!currentService.Success)
            {
                return StatusCode(500, Result.Fail(
                    message: "فشل في العثور على الخدمة",
                    errorType: "ServiceNotFound",
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
                    var fileResult = await _fileService.SaveFileAsync(file, "media/services");
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
            if (currentService.Data.Media != null)
            {
                foreach (var media in currentService.Data.Media)
                {
                    var deleteResult = _fileService.DeleteFile(media.Url);
                    if (!deleteResult.Success)
                    {
                        _logger.LogWarning("Failed to delete media file: {Url}, Error: {Error}", 
                            media.Url, deleteResult.Message);
                    }
                }
            }

            var result = await _mediator.Send(new UpdateServiceAggregateCommand(id, dto, userId));
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث الخدمة مع الوسائط والميزات",
                    errorType: "UpdateServiceAggregateFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result.Ok(
                message: "تم تحديث الخدمة مع الوسائط والميزات بنجاح",
                resultStatus: ResultStatus.Success));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating service aggregate {ServiceId}", id);
            return StatusCode(500, Result.Fail(
                message: $"فشل في تحديث الخدمة: {ex.Message}",
                errorType: "UpdateServiceAggregateFailed",
                resultStatus: ResultStatus.Failed));
        }
    }

    [HttpDelete("aggregate/{id}")]
    public async Task<IActionResult> DeleteAggregate(Guid id)
    {
        try
        {
            // Get current service to delete media files
            var currentService = await _mediator.Send(new GetServiceByIdQuery(id, User.GetId()));
            if (currentService.Success && currentService.Data.Media != null)
            {
                foreach (var media in currentService.Data.Media)
                {
                    var deleteResult = _fileService.DeleteFile(media.Url);
                    if (!deleteResult.Success)
                    {
                        _logger.LogWarning("Failed to delete media file: {Url}, Error: {Error}", 
                            media.Url, deleteResult.Message);
                    }
                }
            }

            var result = await _mediator.Send(new DeleteServiceAggregateCommand(id));
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف الخدمة مع الوسائط والميزات",
                    errorType: "DeleteServiceAggregateFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result.Ok(
                message: "تم حذف الخدمة مع الوسائط والميزات بنجاح",
                resultStatus: ResultStatus.Success));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting service aggregate {ServiceId}", id);
            return StatusCode(500, Result.Fail(
                message: "فشل في حذف الخدمة مع الوسائط والميزات",
                errorType: "DeleteServiceAggregateFailed",
                resultStatus: ResultStatus.Failed));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateServiceDTO dto)
    {
        var userId = User.GetId();
        var result = await _mediator.Send(new UpdateServiceSimpleCommand(id, dto, userId));
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في تحديث الخدمة",
                errorType: "UpdateServiceFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result.Ok(
            message: "تم تحديث الخدمة بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteServiceSimpleCommand(id));
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في حذف الخدمة",
                errorType: "DeleteServiceFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result.Ok(
            message: "تم حذف الخدمة بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpPost("by-ids")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIds([FromBody] IEnumerable<Guid> ids)
    {
        if (ids == null || !ids.Any())
            return BadRequest(Result.Fail("قائمة المعرفات مطلوبة.", "BadRequest", ResultStatus.ValidationError));
        var query = new GetServicesByIdsQuery(ids, new PaginationParameters { PageNumber = 1, PageSize = ids.Count() });
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب الخدمات",
                errorType: "GetServicesByIdsFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<PaginatedResult<ServiceDetailsDTO>>.Ok(
            data: result.Data,
            message: "تم جلب الخدمات بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet("my-services")]
    public async Task<IActionResult> GetMyServices([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.GetId();
        var query = new GetServicesByUserIdQuery(userId, new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize });
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب خدمات المستخدم",
                errorType: "GetServicesByUserIdFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<PaginatedResult<ServiceDTO>>.Ok(
            data: result.Data,
            message: "تم جلب خدمات المستخدم بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchServices([FromQuery] string name, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.Identity.IsAuthenticated ? User.GetId() : Guid.Empty;
        var query = new GetServicesByNameQuery(name, userId, new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize });
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في البحث عن الخدمات",
                errorType: "GetServicesByNameFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<PaginatedResult<ServiceDTO>>.Ok(
            data: result.Data,
            message: "تم البحث عن الخدمات بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetServicesByCategory(Guid categoryId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.Identity.IsAuthenticated ? User.GetId() : Guid.Empty;
        var query = new GetServicesByCategoryQuery(categoryId, userId, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب خدمات الفئة",
                errorType: "GetServicesByCategoryFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<PaginatedResult<ServiceDTO>>.Ok(
            data: result.Data,
            message: "تم جلب خدمات الفئة بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet("price-range")]
    [AllowAnonymous]
    public async Task<IActionResult> GetServicesByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.Identity.IsAuthenticated ? User.GetId() : Guid.Empty;
        var query = new GetServicesByPriceRangeQuery(minPrice, maxPrice, userId, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب الخدمات حسب نطاق السعر",
                errorType: "GetServicesByPriceRangeFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<PaginatedResult<ServiceDTO>>.Ok(
            data: result.Data,
            message: "تم جلب الخدمات حسب نطاق السعر بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet("duration-range")]
    [AllowAnonymous]
    public async Task<IActionResult> GetServicesByDuration([FromQuery] int minDuration, [FromQuery] int maxDuration, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.Identity.IsAuthenticated ? User.GetId() : Guid.Empty;
        var query = new GetServicesByDurationQuery(minDuration, maxDuration, userId, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب الخدمات حسب نطاق المدة",
                errorType: "GetServicesByDurationFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<PaginatedResult<ServiceDTO>>.Ok(
            data: result.Data,
            message: "تم جلب الخدمات حسب نطاق المدة بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet("{serviceId}/base-item-id")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBaseItemIdByServiceId(Guid serviceId)
    {
        var query = new GetBaseItemIdByServiceIdQuery(serviceId);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب معرف العنصر الأساسي",
                errorType: "GetBaseItemIdByServiceIdFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<Guid>.Ok(
            data: result.Data,
            message: "تم جلب معرف العنصر الأساسي بنجاح",
            resultStatus: ResultStatus.Success));
    }
} 