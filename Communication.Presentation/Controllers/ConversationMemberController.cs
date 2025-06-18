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
using Microsoft.AspNetCore.Authorization;

namespace Communication.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
                return StatusCode(500, Result.Fail(
                    message: "فشل في إنشاء عضو المحادثة",
                    errorType: "CreateConversationMemberFailed",
                    resultStatus: ResultStatus.Failed));
            return CreatedAtAction(nameof(GetById), new { id = result.Data }, Result<Guid>.Ok(
                data: result.Data,
                message: "تم إنشاء عضو المحادثة بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<ConversationMemberDTO>>> GetById(Guid id)
        {
            var query = new GetConversationMemberByIdQuery(id);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب عضو المحادثة",
                    errorType: "GetConversationMemberByIdFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<ConversationMemberDTO>.Ok(
                data: result.Data,
                message: "تم جلب عضو المحادثة بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<ConversationMemberDTO>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new Core.Pagination.PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllConversationMembersQuery(parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب أعضاء المحادثة",
                    errorType: "GetAllConversationMembersFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<PaginatedResult<ConversationMemberDTO>>.Ok(
                data: result.Data,
                message: "تم جلب أعضاء المحادثة بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, CreateConversationMemberDTO dto)
        {
            var command = new UpdateConversationMemberCommand(id, dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث عضو المحادثة",
                    errorType: "UpdateConversationMemberFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم تحديث عضو المحادثة بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteConversationMemberCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف عضو المحادثة",
                    errorType: "DeleteConversationMemberFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم حذف عضو المحادثة بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
} 