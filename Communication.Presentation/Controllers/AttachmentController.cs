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
using Microsoft.AspNetCore.Authorization;

namespace Communication.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
                return StatusCode(500, Result.Fail(
                    message: "فشل في إنشاء المرفق",
                    errorType: "CreateAttachmentFailed",
                    resultStatus: ResultStatus.Failed));
            return CreatedAtAction(nameof(GetById), new { id = result.Data }, Result<Guid>.Ok(
                data: result.Data,
                message: "تم إنشاء المرفق بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<AttachmentDTO>>> GetById(Guid id)
        {
            var query = new GetAttachmentByIdQuery(id);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب المرفق",
                    errorType: "GetAttachmentByIdFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<AttachmentDTO>.Ok(
                data: result.Data,
                message: "تم جلب المرفق بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<AttachmentDTO>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllAttachmentsQuery(parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب المرفقات",
                    errorType: "GetAllAttachmentsFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<PaginatedResult<AttachmentDTO>>.Ok(
                data: result.Data,
                message: "تم جلب المرفقات بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, CreateAttachmentDTO dto)
        {
            var command = new UpdateAttachmentCommand(id, dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث المرفق",
                    errorType: "UpdateAttachmentFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم تحديث المرفق بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteAttachmentCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف المرفق",
                    errorType: "DeleteAttachmentFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم حذف المرفق بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
} 