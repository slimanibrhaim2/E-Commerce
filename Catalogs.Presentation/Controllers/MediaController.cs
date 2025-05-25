using Microsoft.AspNetCore.Mvc;
using MediatR;
using Catalogs.Application.Commands.AddMedia;
using Catalogs.Application.Commands.DeleteMedia;
using Catalogs.Application.DTOs;
using Microsoft.Extensions.Logging;
using Catalogs.Application.Queries.GetMediaById;
using Catalogs.Application.Commands.UpdateMedia;

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MediaController> _logger;

        public MediaController(IMediator mediator, ILogger<MediaController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("{itemId}")]
        public async Task<ActionResult<Guid>> AddMedia(Guid itemId, [FromBody] CreateMediaDTO dto)
        {
            var command = new AddMediaCommand(itemId, dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromBody] CreateMediaDTO dto)
        {
            var command = new UpdateMediaCommand(id, dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return BadRequest(result);

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var command = new DeleteMediaCommand(id);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return BadRequest(result);

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MediaDTO>> GetById(Guid id)
        {
            var query = new GetMediaByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return NotFound(result);

            return Ok(result.Data);
        }
    }
} 