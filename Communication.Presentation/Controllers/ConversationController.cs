using Microsoft.AspNetCore.Mvc;
using MediatR;
using Communication.Application.Commands.CreateConversation;
using Communication.Application.Commands.UpdateConversation;
using Communication.Application.Commands.DeleteConversation;
using Communication.Application.Queries.GetConversationById;
using Communication.Application.Queries.GetAllConversations;
using Communication.Application.DTOs;
using Microsoft.Extensions.Logging;
using Core.Result;

namespace Communication.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ConversationController> _logger;

        public ConversationController(IMediator mediator, ILogger<ConversationController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<Result<List<ConversationDTO>>>> GetAllByUser(Guid userId)
        {
            var query = new GetAllConversationsQuery(userId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> Create(CreateConversationDTO dto)
        {
            var command = new CreateConversationCommand(dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<ConversationDTO>>> GetById(Guid id)
        {
            var query = new GetConversationByIdQuery(id);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, CreateConversationDTO dto)
        {
            var command = new UpdateConversationCommand(id, dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteConversationCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
} 