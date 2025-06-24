using Microsoft.AspNetCore.Mvc;
using MediatR;
using Communication.Application.DTOs;
using Microsoft.Extensions.Logging;
using Core.Result;
using Communication.Application.Commands.UpdateComment;
using Communication.Application.Commands.DeleteComment;
using Communication.Application.Queries.GetCommentById;
using Communication.Application.Queries.GetAllComments;
using Communication.Application.Queries.GetAllCommentsByBaseItemId;
using Core.Pagination;
using Communication.Application.Commands.AddCommentAggregate;
using Communication.Application.Commands.UpdateCommentAggregate;
using Communication.Application.Commands.DeleteCommentAggregate;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;

namespace Communication.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CommentController> _logger;

        public CommentController(IMediator mediator, ILogger<CommentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<CommentDTO>>> GetById(Guid id)
        {
            var query = new GetCommentByIdQuery(id);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب التعليق",
                    errorType: "GetCommentByIdFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<CommentDTO>.Ok(
                data: result.Data,
                message: "تم جلب التعليق بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<CommentDTO>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllCommentsQuery(parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب التعليقات",
                    errorType: "GetAllCommentsFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<PaginatedResult<CommentDTO>>.Ok(
                data: result.Data,
                message: "تم جلب التعليقات بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, AddCommentAggregateDTO dto)
        {
            var userId = User.GetId();
            var command = new UpdateCommentCommand(id, dto, userId);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث التعليق",
                    errorType: "UpdateCommentFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم تحديث التعليق بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteCommentCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف التعليق",
                    errorType: "DeleteCommentFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم حذف التعليق بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("baseitem/{baseItemId}")]
        public async Task<ActionResult<Result<List<CommentDTO>>>> GetAllByBaseItemId(Guid baseItemId)
        {
            var query = new GetAllCommentsByBaseItemIdQuery(baseItemId);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب التعليقات",
                    errorType: "GetAllCommentsByBaseItemIdFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<List<CommentDTO>>.Ok(
                data: result.Data,
                message: "تم جلب التعليقات بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpPost("aggregate")]
        public async Task<IActionResult> AddCommentAggregate([FromBody] AddCommentAggregateDTO dto)
        {
            var userId = User.GetId();
            var result = await _mediator.Send(new AddCommentAggregateCommand(dto, userId));
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في إضافة التعليق",
                    errorType: "AddCommentAggregateFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result.Ok(
                message: "تم إضافة التعليق بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpPut("aggregate/{id}")]
        public async Task<IActionResult> UpdateCommentAggregate(Guid id, [FromBody] AddCommentAggregateDTO dto)
        {
            var userId = User.GetId();
            var result = await _mediator.Send(new UpdateCommentAggregateCommand(id, dto, userId));
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث التعليق",
                    errorType: "UpdateCommentAggregateFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result.Ok(
                message: "تم تحديث التعليق بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("aggregate/{id}")]
        public async Task<IActionResult> DeleteCommentAggregate(Guid id)
        {
            var result = await _mediator.Send(new DeleteCommentAggregateCommand(id));
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف التعليق",
                    errorType: "DeleteCommentAggregateFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result.Ok(
                message: "تم حذف التعليق بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
} 