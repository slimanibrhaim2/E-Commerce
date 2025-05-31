using Microsoft.AspNetCore.Mvc;
using MediatR;
using Catalogs.Application.DTOs;
using Catalogs.Application.Queries.GetServicesByUserId;
using Catalogs.Application.Queries.GetServicesByName;
using Catalogs.Application.Queries.GetServicesByCategory;
using Core.Pagination;
using Core.Result;

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ServiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<Result<PaginatedResult<ServiceDTO>>>> GetByUserId(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetServicesByUserIdQuery(userId, parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<Result<PaginatedResult<ServiceDTO>>>> GetByName(
            [FromQuery] string name,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetServicesByNameQuery(name, parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<Result<PaginatedResult<ServiceDTO>>>> GetByCategory(Guid categoryId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetServicesByCategoryQuery(categoryId, pageNumber, pageSize);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
} 