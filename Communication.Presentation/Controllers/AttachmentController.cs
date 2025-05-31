using Microsoft.AspNetCore.Mvc;
using MediatR;
using Communication.Application.DTOs;
using Microsoft.Extensions.Logging;
using Core.Result;
using Communication.Application.Commands.CreateAttachment;
using Communication.Application.Commands.UpdateAttachment;
using Communication.Application.Commands.DeleteAttachment;
using Communication.Application.Queries.GetAttachmentById;
using Communication.Application.Queries.GetAllAttachments;
using Core.Pagination;

namespace Communication.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttachmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AttachmentController> _logger;

        public AttachmentController(IMediator mediator, ILogger<AttachmentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> Create(CreateAttachmentDTO dto)
        {
            var command = new CreateAttachmentCommand(dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<AttachmentDTO>>> GetById(Guid id)
        {
            var query = new GetAttachmentByIdQuery(id);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<AttachmentDTO>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllAttachmentsQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, CreateAttachmentDTO dto)
        {
            var command = new UpdateAttachmentCommand(id, dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteAttachmentCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
} 