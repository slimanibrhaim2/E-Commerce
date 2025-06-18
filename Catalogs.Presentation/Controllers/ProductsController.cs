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
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;
using Catalogs.Application.Queries.GetProductsByUserId;
using Catalogs.Application.Queries.GetProductsByCategory;
using Catalogs.Application.Queries.GetProductsByName;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;

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
        dto.UserId = userId;
        var result = await _mediator.Send(new CreateProductCommand(dto));
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في إنشاء المنتج",
                errorType = "CreateProductFailed"
            });
        return CreatedAtAction("GetById", new { id = result.Data }, new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم إنشاء المنتج بنجاح",
            errorType = (string)null,
            data = result.Data
        });
    }

    [HttpPost("aggregate")]
    public async Task<IActionResult> CreateAggregate([FromBody] CreateProductAggregateDTO dto)
    {
        var userId = User.GetId();
        dto.UserId = userId;
        var result = await _mediator.Send(new CreateProductAggregateCommand(dto));
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في إنشاء المنتج مع الوسائط والميزات",
                errorType = "CreateProductAggregateFailed"
            });
        return CreatedAtAction("GetById", new { id = result.Data }, new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم إنشاء المنتج مع الوسائط والميزات بنجاح",
            errorType = (string)null,
            data = result.Data
        });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var pagination = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
        var query = new GetAllProductsQuery(pagination);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في جلب المنتجات",
                errorType = "GetAllFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم جلب المنتجات بنجاح",
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
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في جلب المنتج",
                errorType = "GetByIdFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم جلب المنتج بنجاح",
            errorType = (string)null,
            data = result.Data
        });
    }

    [HttpPut("aggregate/{id}")]
    public async Task<IActionResult> UpdateAggregate(Guid id, [FromBody] CreateProductAggregateDTO dto)
    {
        var userId = User.GetId();
        dto.UserId = userId;
        var result = await _mediator.Send(new UpdateProductAggregateCommand(id, dto));
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في تحديث المنتج مع الوسائط والميزات",
                errorType = "UpdateProductAggregateFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم تحديث المنتج مع الوسائط والميزات بنجاح",
            errorType = (string)null
        });
    }

    [HttpDelete("aggregate/{id}")]
    public async Task<IActionResult> DeleteAggregate(Guid id)
    {
        var result = await _mediator.Send(new DeleteProductAggregateCommand(id));
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في حذف المنتج مع الوسائط والميزات",
                errorType = "DeleteProductAggregateFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم حذف المنتج مع الوسائط والميزات بنجاح",
            errorType = (string)null
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductDTO dto)
    {
        var userId = User.GetId();
        dto.UserId = userId;
        var result = await _mediator.Send(new UpdateProductSimpleCommand(id, dto));
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في تحديث المنتج",
                errorType = "UpdateProductFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم تحديث المنتج بنجاح",
            errorType = (string)null
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteProductSimpleCommand(id));
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في حذف المنتج",
                errorType = "DeleteProductFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم حذف المنتج بنجاح",
            errorType = (string)null
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
        var result = await _mediator.Send(new GetProductsByIdsQuery(ids));
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم جلب المنتجات بنجاح",
            errorType = (string)null,
            data = result
        });
    }

    [HttpGet("my-products")]
    public async Task<IActionResult> GetMyProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.GetId();
        var query = new GetProductsByUserIdQuery(userId, new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize });
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في جلب منتجات المستخدم",
                errorType = "GetProductsByUserIdFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم جلب منتجات المستخدم بنجاح",
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

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchProducts([FromQuery] string name)
    {
        var query = new GetProductsByNameQuery(name, new PaginationParameters { PageNumber = 1, PageSize = 10 });
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في البحث عن المنتجات",
                errorType = "GetProductsByNameFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم البحث عن المنتجات بنجاح",
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
    public async Task<IActionResult> GetProductsByCategory(Guid categoryId)
    {
        var query = new GetProductsByCategoryQuery(categoryId, 1, 10);
        var result = await _mediator.Send(query);
        if (!result.Success)
            return StatusCode(500, new
            {
                resultStatus = (int)ResultStatus.Failed,
                success = false,
                message = "فشل في جلب منتجات الفئة",
                errorType = "GetProductsByCategoryFailed"
            });
        return Ok(new
        {
            resultStatus = (int)ResultStatus.Success,
            success = true,
            message = "تم جلب منتجات الفئة بنجاح",
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