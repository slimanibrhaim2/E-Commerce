using Microsoft.AspNetCore.Mvc;
using MediatR;
using Catalogs.Application.DTOs;
using Microsoft.Extensions.Logging;
using Catalogs.Application.Queries.GetAllBaseItemsByUserId;

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
        public async Task<ActionResult<List<BaseItemDTO>>> GetAllByUserId(Guid userId)
        {
            var query = new GetAllBaseItemsByUserIdQuery(userId);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result.Data);
        }
    }
} 