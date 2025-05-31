using Microsoft.AspNetCore.Mvc;
using MediatR;
using Catalogs.Application.DTOs;
using Catalogs.Application.Queries.GetProductsByUserId;
using Catalogs.Application.Queries.GetProductsByName;
using Catalogs.Application.Queries.GetProductsByCategory;
using Core.Pagination;
using Core.Result;

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<Result<PaginatedResult<ProductDTO>>>> GetByUserId(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetProductsByUserIdQuery(userId, parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<Result<PaginatedResult<ProductDTO>>>> GetByName(
            [FromQuery] string name,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetProductsByNameQuery(name, parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<Result<PaginatedResult<ProductDTO>>>> GetByCategory(Guid categoryId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetProductsByCategoryQuery(categoryId, pageNumber, pageSize);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
} 