using Microsoft.AspNetCore.Mvc;
using MediatR;
using Catalogs.Application.Commands.AddFeature;
using Catalogs.Application.Commands.UpdateFeature;
using Catalogs.Application.Commands.DeleteFeature;
using Catalogs.Application.DTOs;
using Microsoft.Extensions.Logging;
using Catalogs.Application.Queries.GetFeatureById;

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeatureController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FeatureController> _logger;

        public FeatureController(IMediator mediator, ILogger<FeatureController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("{entityId}")]
        public async Task<ActionResult<Guid>> AddFeature(Guid entityId, [FromBody] CreateFeatureDTO dto)
        {
            var command = new AddFeatureCommand(entityId, dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, [FromBody] CreateFeatureDTO dto)
        {
            var command = new UpdateFeatureCommand(id, dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return BadRequest(result);

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var command = new DeleteFeatureCommand(id);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return BadRequest(result);

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FeatureDTO>> GetById(Guid id)
        {
            var query = new GetFeatureByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return NotFound(result);

            return Ok(result.Data);
        }
    }
} 