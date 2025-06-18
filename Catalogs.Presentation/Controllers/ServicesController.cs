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
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;
using Catalogs.Application.Queries.GetServicesByUserId;
using Catalogs.Application.Queries.GetServicesByName;
using Catalogs.Application.Queries.GetServicesByCategory;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;

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
    public async Task<IActionResult> Create([FromBody] ServiceDTO dto)
    {
        var userId = User.GetId();
        dto.UserId = userId;
        var result = await _mediator.Send(new CreateServiceCommand(dto));
        if (!result.Success || result.Data == Guid.Empty)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في إنشاء الخدمة",
                errorType = "CreateServiceFailed"
            });
        return CreatedAtAction("GetById", new { id = result.Data }, new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم إنشاء الخدمة بنجاح",
            errorType = (string)null,
            data = result.Data
        });
    }

    [HttpPost("aggregate")]
    public async Task<IActionResult> CreateAggregate([FromBody] CreateServiceAggregateDTO dto)
    {
        var userId = User.GetId();
        dto.UserId = userId;
        var result = await _mediator.Send(new CreateServiceAggregateCommand(dto));
        if (!result.Success || result.Data == Guid.Empty)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في إنشاء الخدمة مع الوسائط والميزات",
                errorType = "CreateServiceAggregateFailed"
            });
        return CreatedAtAction("GetById", new { id = result.Data }, new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم إنشاء الخدمة مع الوسائط والميزات بنجاح",
            errorType = (string)null,
            data = result.Data
        });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
        var query = new GetAllServicesQuery(pagination);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في جلب الخدمات",
                errorType = "GetAllFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم جلب الخدمات بنجاح",
            errorType = (string)null,
            data = result.Data.Data,
            pagination = new
            {
                pageNumber = result.Data.PageNumber,
                pageSize = result.Data.PageSize,
                totalPages = result.Data.TotalPages,
                totalCount = result.Data.TotalCount,
                hasPreviousPage = result.Data.HasPreviousPage,
                hasNextPage = result.Data.HasNextPage
            }
        });
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetServiceByIdQuery(id);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في جلب الخدمة",
                errorType = "GetByIdFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم جلب الخدمة بنجاح",
            errorType = (string)null,
            data = result.Data
        });
    }

    [HttpPut("aggregate/{id}")]
    public async Task<IActionResult> UpdateAggregate(Guid id, [FromBody] CreateServiceAggregateDTO dto)
    {
        var userId = User.GetId();
        dto.UserId = userId;
        var result = await _mediator.Send(new UpdateServiceAggregateCommand(id, dto));
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في تحديث الخدمة مع الوسائط والميزات",
                errorType = "UpdateServiceAggregateFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم تحديث الخدمة مع الوسائط والميزات بنجاح",
            errorType = (string)null
        });
    }

    [HttpDelete("aggregate/{id}")]
    public async Task<IActionResult> DeleteAggregate(Guid id)
    {
        var result = await _mediator.Send(new DeleteServiceAggregateCommand(id));
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في حذف الخدمة مع الوسائط والميزات",
                errorType = "DeleteServiceAggregateFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم حذف الخدمة مع الوسائط والميزات بنجاح",
            errorType = (string)null
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ServiceDTO dto)
    {
        var userId = User.GetId();
        dto.UserId = userId;
        var result = await _mediator.Send(new UpdateServiceSimpleCommand(id, dto));
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في تحديث الخدمة",
                errorType = "UpdateServiceFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم تحديث الخدمة بنجاح",
            errorType = (string)null
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteServiceSimpleCommand(id));
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في حذف الخدمة",
                errorType = "DeleteServiceFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم حذف الخدمة بنجاح"
        });
    }

    [HttpPost("by-ids")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByIds([FromBody] IEnumerable<Guid> ids)
    {
        if (ids == null || !ids.Any())
            return BadRequest(new
            {
                resultStatus = (int)ResultStatus.ValidationError,
                success = false,
                message = "قائمة المعرفات مطلوبة.",
                errorType = "BadRequest"
            });
        var result = await _mediator.Send(new GetServicesByIdsQuery(ids));
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم جلب الخدمات بنجاح",
            data = result
        });
    }

    [HttpGet("my-services")]
    public async Task<IActionResult> GetMyServices([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.GetId();
        var query = new GetServicesByUserIdQuery(userId, new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize });
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في جلب خدمات المستخدم",
                errorType = "GetServicesByUserIdFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم جلب خدمات المستخدم بنجاح",
            data = result.Data.Data,
            pagination = new
            {
                pageNumber = result.Data.PageNumber,
                pageSize = result.Data.PageSize,
                totalPages = result.Data.TotalPages,
                totalCount = result.Data.TotalCount,
                hasPreviousPage = result.Data.HasPreviousPage,
                hasNextPage = result.Data.HasNextPage
            }
        });
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchServices([FromQuery] string name)
    {
        var query = new GetServicesByNameQuery(name, new PaginationParameters { PageNumber = 1, PageSize = 10 });
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في البحث عن الخدمات",
                errorType = "GetServicesByNameFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم البحث عن الخدمات بنجاح",
            errorType = (string)null,
            data = result.Data.Data,
            pagination = new
            {
                pageNumber = result.Data.PageNumber,
                pageSize = result.Data.PageSize,
                totalPages = result.Data.TotalPages,
                totalCount = result.Data.TotalCount,
                hasPreviousPage = result.Data.HasPreviousPage,
                hasNextPage = result.Data.HasNextPage
            }
        });
    }

    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetServicesByCategory(Guid categoryId)
    {
        var query = new GetServicesByCategoryQuery(categoryId, 1, 10);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في جلب خدمات الفئة",
                errorType = "GetServicesByCategoryFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم جلب خدمات الفئة بنجاح",
            errorType = (string)null,
            data = result.Data.Data,
            pagination = new
            {
                pageNumber = result.Data.PageNumber,
                pageSize = result.Data.PageSize,
                totalPages = result.Data.TotalPages,
                totalCount = result.Data.TotalCount,
                hasPreviousPage = result.Data.HasPreviousPage,
                hasNextPage = result.Data.HasNextPage
            }
        });
    }
} 