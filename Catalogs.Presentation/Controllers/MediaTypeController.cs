using Microsoft.AspNetCore.Mvc;
using MediatR;
using Catalogs.Application.DTOs;
using Catalogs.Application.Commands.CreateMediaType;
using Catalogs.Application.Commands.UpdateMediaType;
using Catalogs.Application.Commands.DeleteMediaType;
using Microsoft.Extensions.Logging;
using Catalogs.Application.Queries.GetAllMediaTypes;
using Catalogs.Application.Queries.GetMediaTypeById;

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaTypeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MediaTypeController> _logger;

        public MediaTypeController(IMediator mediator, ILogger<MediaTypeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<MediaTypeDTO>>> GetAll()
        {
            var query = new GetAllMediaTypesQuery();
            var result = await _mediator.Send(query);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MediaTypeDTO>> GetById(Guid id)
        {
            var query = new GetMediaTypeByIdQuery(id);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return NotFound(result);
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CreateMediaTypeDTO dto)
        {
            var command = new CreateMediaTypeCommand(dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            // return CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data);
            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, CreateMediaTypeDTO dto)
        {
            var command = new UpdateMediaTypeCommand(id, dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var command = new DeleteMediaTypeCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result.Data);
        }
    }
} 