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

namespace Catalogs.Presentation.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
        => _mediator = mediator;

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
    public async Task<IActionResult> CreateAggregate([FromBody] CreateProductAggregateDTO dto)
    {
        var userId = User.GetId();
        var result = await _mediator.Send(new CreateProductAggregateCommand(dto, userId));
        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في إنشاء المنتج مع الوسائط والميزات",
                errorType: "CreateProductAggregateFailed",
                resultStatus: ResultStatus.Failed));
        return CreatedAtAction("GetById", new { id = result.Data }, Result<Guid>.Ok(
            data: result.Data,
            message: "تم إنشاء المنتج مع الوسائط والميزات بنجاح",
            resultStatus: ResultStatus.Success));
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
    public async Task<IActionResult> UpdateAggregate(Guid id, [FromBody] CreateProductAggregateDTO dto)
    {
        var userId = User.GetId();
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