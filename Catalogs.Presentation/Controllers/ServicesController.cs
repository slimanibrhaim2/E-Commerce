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

namespace Catalogs.Presentation.Controllers;

[ApiController]
[Route("api/services")]
[Authorize]
public class ServicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServicesController(IMediator mediator)
        => _mediator = mediator;

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
    public async Task<IActionResult> CreateAggregate([FromBody] CreateServiceAggregateDTO dto)
    {
        var userId = User.GetId();
        var result = await _mediator.Send(new CreateServiceAggregateCommand(dto, userId));
        if (!result.Success || result.Data == Guid.Empty)
            return StatusCode(500, Result.Fail(
                message: "فشل في إنشاء الخدمة مع الوسائط والميزات",
                errorType: "CreateServiceAggregateFailed",
                resultStatus: ResultStatus.Failed));
        return CreatedAtAction("GetById", new { id = result.Data }, Result<Guid>.Ok(
            data: result.Data,
            message: "تم إنشاء الخدمة مع الوسائط والميزات بنجاح",
            resultStatus: ResultStatus.Success));
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
    public async Task<IActionResult> UpdateAggregate(Guid id, [FromBody] CreateServiceAggregateDTO dto)
    {
        var userId = User.GetId();
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

    [HttpDelete("aggregate/{id}")]
    public async Task<IActionResult> DeleteAggregate(Guid id)
    {
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