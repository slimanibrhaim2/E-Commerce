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

namespace Catalogs.Presentation.Controllers;

[ApiController]
[Route("api/services")]
public class ServicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ServicesController(IMediator mediator)
        => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ServiceDto dto)
    {
        var result = await _mediator.Send(new CreateServiceCommand(dto));
        if (!result.Success || result.Data == Guid.Empty)
            return StatusCode(500, Result.Fail(
                message: "فشل في إنشاء الخدمة",
                errorType: "CreateServiceFailed",
                resultStatus: ResultStatus.Failed));
        return CreatedAtAction("GetById", new { id = result.Data }, Result.Ok(
            message: "تم إنشاء الخدمة بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpPost("aggregate")]
    public async Task<IActionResult> CreateAggregate([FromBody] CreateServiceAggregateDTO dto)
    {
        var result = await _mediator.Send(new CreateServiceAggregateCommand(dto));
        if (!result.Success || result.Data == Guid.Empty)
            return StatusCode(500, Result.Fail(
                message: "فشل في إنشاء الخدمة مع الوسائط والميزات",
                errorType: "CreateServiceAggregateFailed",
                resultStatus: ResultStatus.Failed));
        return CreatedAtAction("GetById", new { id = result.Data }, Result.Ok(
            message: "تم إنشاء الخدمة مع الوسائط والميزات بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
        var query = new GetAllServicesQuery(pagination);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب الخدمات",
                errorType: "GetAllFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result<PaginatedResult<ServiceDto>>.Ok(
            data: result.Data,
            message: "تم جلب الخدمات بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetServiceByIdQuery(id);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب الخدمة",
                errorType: "GetByIdFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(result);
    }

    [HttpPut("aggregate/{id}")]
    public async Task<IActionResult> UpdateAggregate(Guid id, [FromBody] CreateServiceAggregateDTO dto)
    {
        var result = await _mediator.Send(new UpdateServiceAggregateCommand(id, dto));
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
    public async Task<IActionResult> Update(Guid id, [FromBody] ServiceDto dto)
    {
        var result = await _mediator.Send(new UpdateServiceSimpleCommand(id, dto));
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
} 