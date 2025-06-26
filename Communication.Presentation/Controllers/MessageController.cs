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
using System.Security.Claims;

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

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                throw new UnauthorizedAccessException("Invalid user token");
            }
            return userId;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> Create(CreateMessageDTO dto)
        {
            try
            {
                dto.SenderId = GetCurrentUserId(); // Always set sender from token
                var command = new CreateMessageCommand(dto);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في إنشاء الرسالة",
                        errorType: "CreateMessageFailed",
                        resultStatus: ResultStatus.Failed));
                return CreatedAtAction(nameof(GetById), new { id = result.Data }, Result<Guid>.Ok(
                    data: result.Data,
                    message: "تم إنشاء الرسالة بنجاح",
                    resultStatus: ResultStatus.Success));
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(401, Result.Fail(
                    message: "غير مصرح",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError));
            }
        }

        [HttpPost("aggregate")]
        public async Task<ActionResult<Result<Guid>>> AddMessageAggregate(AddMessageAggregateDTO dto)
        {
            try
            {
                dto.SenderId = GetCurrentUserId(); // Always set sender from token
                var command = new AddMessageAggregateCommand(dto);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في إنشاء الرسالة",
                        errorType: "CreateMessageFailed",
                        resultStatus: ResultStatus.Failed));
                return CreatedAtAction(nameof(GetById), new { id = result.Data }, Result<Guid>.Ok(
                    data: result.Data,
                    message: "تم إنشاء الرسالة بنجاح",
                    resultStatus: ResultStatus.Success));
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(401, Result.Fail(
                    message: "غير مصرح",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<MessageDTO>>> GetById(Guid id)
        {
            var query = new GetMessageByIdQuery(id);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب الرسالة",
                    errorType: "GetMessageFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<MessageDTO>>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            var query = new GetAllMessagesQuery(parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب الرسائل",
                    errorType: "GetMessagesFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, CreateMessageDTO dto)
        {
            try
            {
                dto.SenderId = GetCurrentUserId(); // Always set sender from token
                var command = new UpdateMessageCommand(id, dto);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في تحديث الرسالة",
                        errorType: "UpdateMessageFailed",
                        resultStatus: ResultStatus.Failed));
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(401, Result.Fail(
                    message: "غير مصرح",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError));
            }
        }

        [HttpPut("aggregate/{id}")]
        public async Task<ActionResult<Result<Guid>>> UpdateMessageAggregate(Guid id, AddMessageAggregateDTO dto)
        {
            try
            {
                dto.SenderId = GetCurrentUserId(); // Always set sender from token
                var command = new UpdateMessageAggregateCommand(id, dto);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في تحديث الرسالة",
                        errorType: "UpdateMessageFailed",
                        resultStatus: ResultStatus.Failed));
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(401, Result.Fail(
                    message: "غير مصرح",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteMessageCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف الرسالة",
                    errorType: "DeleteMessageFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(result);
        }

        [HttpDelete("aggregate/{id}")]
        public async Task<ActionResult<Result<bool>>> DeleteMessageAggregate(Guid id)
        {
            var command = new DeleteMessageAggregateCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف الرسالة",
                    errorType: "DeleteMessageFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(result);
        }
    }
} 