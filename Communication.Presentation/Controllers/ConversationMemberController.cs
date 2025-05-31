using Microsoft.AspNetCore.Mvc;
using MediatR;
using Communication.Application.DTOs;
using Microsoft.Extensions.Logging;
using Core.Result;
using Communication.Application.Commands.CreateConversationMember;
using Communication.Application.Commands.UpdateConversationMember;
using Communication.Application.Commands.DeleteConversationMember;
using Communication.Application.Queries.GetConversationMemberById;
using Communication.Application.Queries.GetAllConversationMembers;
using Core.Pagination;

namespace Communication.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversationMemberController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ConversationMemberController> _logger;

        public ConversationMemberController(IMediator mediator, ILogger<ConversationMemberController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> Create(CreateConversationMemberDTO dto)
        {
            var command = new CreateConversationMemberCommand(dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<ConversationMemberDTO>>> GetById(Guid id)
        {
            var query = new GetConversationMemberByIdQuery(id);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<ConversationMemberDTO>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new Core.Pagination.PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllConversationMembersQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, CreateConversationMemberDTO dto)
        {
            var command = new UpdateConversationMemberCommand(id, dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteConversationMemberCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
    }
} 