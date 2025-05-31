using Microsoft.AspNetCore.Mvc;
using MediatR;
using Catalogs.Application.DTOs;
using Microsoft.Extensions.Logging;
using Catalogs.Application.Queries.GetAllBaseItemsByUserId;
using Core.Pagination;
using Core.Result;

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseItemController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BaseItemController> _logger;

        public BaseItemController(IMediator mediator, ILogger<BaseItemController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<Result<PaginatedResult<BaseItemDTO>>>> GetAllByUserId(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllBaseItemsByUserIdQuery(userId, parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
} 