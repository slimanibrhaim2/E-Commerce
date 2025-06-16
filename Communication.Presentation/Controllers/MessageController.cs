using Microsoft.AspNetCore.Mvc;
using MediatR;
using Communication.Application.DTOs;
using Microsoft.Extensions.Logging;
using Core.Result;
using Communication.Application.Commands.CreateMessage;
using Communication.Application.Commands.UpdateMessage;
using Communication.Application.Commands.DeleteMessage;
using Communication.Application.Queries.GetMessageById;
using Communication.Application.Queries.GetAllMessages;
using Core.Pagination;
using Communication.Application.Commands.AddMessageAggregate;
using Communication.Application.Commands.UpdateMessageAggregate;
using Communication.Application.Commands.DeleteMessageAggregate;
using Microsoft.AspNetCore.Authorization;

namespace Communication.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MessageController> _logger;

        public MessageController(IMediator mediator, ILogger<MessageController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> Create(CreateMessageDTO dto)
        {
            var command = new CreateMessageCommand(dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<MessageDTO>>> GetById(Guid id)
        {
            var query = new GetMessageByIdQuery(id);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<MessageDTO>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllMessagesQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, CreateMessageDTO dto)
        {
            var command = new UpdateMessageCommand(id, dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteMessageCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("aggregate")]
        public async Task<IActionResult> AddMessageAggregate([FromBody] AddMessageAggregateDTO dto)
        {
            var result = await _mediator.Send(new AddMessageAggregateCommand(dto));
            if (!result.Success)
                return StatusCode(500, result);
            return Ok(result);
        }

        [HttpPut("aggregate/{id}")]
        public async Task<IActionResult> UpdateMessageAggregate(Guid id, [FromBody] AddMessageAggregateDTO dto)
        {
            var result = await _mediator.Send(new UpdateMessageAggregateCommand(id, dto));
            if (!result.Success)
                return BadRequest(result.ErrorType);

            return Ok(result.Data);
        }

        [HttpDelete("aggregate/{id}")]
        public async Task<IActionResult> DeleteMessageAggregate(Guid id)
        {
            var result = await _mediator.Send(new DeleteMessageAggregateCommand(id));
            if (!result.Success)
                return BadRequest(result.ErrorType);

            return Ok(result.Data);
        }
    }
} 