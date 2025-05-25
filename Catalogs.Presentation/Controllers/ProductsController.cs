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

namespace Catalogs.Presentation.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
        => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDTO dto)
    {
        var result = await _mediator.Send(new CreateProductCommand(dto));
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في إنشاء المنتج",
                errorType: "CreateProductFailed",
                resultStatus: ResultStatus.Failed));
        return CreatedAtAction("GetById", new { id = result.Data }, Result.Ok(
            message: "تم إنشاء المنتج بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpPost("aggregate")]
    public async Task<IActionResult> CreateAggregate([FromBody] CreateProductAggregateDTO dto)
    {
        var result = await _mediator.Send(new CreateProductAggregateCommand(dto));
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في إنشاء المنتج مع الوسائط والميزات",
                errorType: "CreateProductAggregateFailed",
                resultStatus: ResultStatus.Failed));
        return CreatedAtAction("GetById", new { id = result.Data }, Result.Ok(
            message: "تم إنشاء المنتج مع الوسائط والميزات بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
        var query = new GetAllProductsQuery(pagination);
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
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب المنتج",
                errorType: "GetByIdFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(result);
    }

    [HttpPut("aggregate/{id}")]
    public async Task<IActionResult> UpdateAggregate(Guid id, [FromBody] CreateProductAggregateDTO dto)
    {
        var result = await _mediator.Send(new UpdateProductAggregateCommand(id, dto));
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في تحديث المنتج مع الوسائط والميزات",
                errorType: "UpdateProductAggregateFailed",
                resultStatus: ResultStatus.Failed));
        return Ok(Result.Ok(
            message: "تم تحديث المنتج مع الوسائط والميزات بنجاح",
            resultStatus: ResultStatus.Success));
    }

    [HttpDelete("aggregate/{id}")]
    public async Task<IActionResult> DeleteAggregate(Guid id)
    {
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductDTO dto)
    {
        var result = await _mediator.Send(new UpdateProductSimpleCommand(id, dto));
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
} 