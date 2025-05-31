using Microsoft.AspNetCore.Mvc;
using MediatR;
using Communication.Application.DTOs;
using Microsoft.Extensions.Logging;
using Core.Result;
using Communication.Application.Commands.CreateComment;
using Communication.Application.Commands.UpdateComment;
using Communication.Application.Commands.DeleteComment;
using Communication.Application.Queries.GetCommentById;
using Communication.Application.Queries.GetAllComments;
using Communication.Application.Queries.GetAllCommentsByBaseItemId;
using Core.Pagination;

namespace Communication.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CommentController> _logger;

        public CommentController(IMediator mediator, ILogger<CommentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> Create(CreateCommentDTO dto)
        {
            var command = new CreateCommentCommand(dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<CommentDTO>>> GetById(Guid id)
        {
            var query = new GetCommentByIdQuery(id);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<CommentDTO>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllCommentsQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, CreateCommentDTO dto)
        {
            var command = new UpdateCommentCommand(id, dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteCommentCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("baseitem/{baseItemId}")]
        public async Task<ActionResult<Result<List<CommentDTO>>>> GetAllByBaseItemId(Guid baseItemId)
        {
            var query = new GetAllCommentsByBaseItemIdQuery(baseItemId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 