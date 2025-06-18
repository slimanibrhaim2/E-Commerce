using Microsoft.AspNetCore.Mvc;
using MediatR;
using Communication.Application.DTOs;
using Microsoft.Extensions.Logging;
using Core.Result;
using Communication.Application.Commands.CreateAttachmentType;
using Communication.Application.Commands.UpdateAttachmentType;
using Communication.Application.Commands.DeleteAttachmentType;
using Communication.Application.Queries.GetAttachmentTypeById;
using Communication.Application.Queries.GetAllAttachmentTypes;
using Core.Pagination;
using Microsoft.AspNetCore.Authorization;

namespace Communication.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttachmentTypeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AttachmentTypeController> _logger;

        public AttachmentTypeController(IMediator mediator, ILogger<AttachmentTypeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> Create(CreateAttachmentTypeDTO dto)
        {
            var command = new CreateAttachmentTypeCommand(dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في إنشاء نوع المرفق",
                    errorType: "CreateAttachmentTypeFailed",
                    resultStatus: ResultStatus.Failed));
            return CreatedAtAction(nameof(GetById), new { id = result.Data }, Result<Guid>.Ok(
                data: result.Data,
                message: "تم إنشاء نوع المرفق بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<AttachmentTypeDTO>>> GetById(Guid id)
        {
            var query = new GetAttachmentTypeByIdQuery(id);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب نوع المرفق",
                    errorType: "GetAttachmentTypeByIdFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<AttachmentTypeDTO>.Ok(
                data: result.Data,
                message: "تم جلب نوع المرفق بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<AttachmentTypeDTO>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllAttachmentTypesQuery(parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب أنواع المرفقات",
                    errorType: "GetAllAttachmentTypesFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<PaginatedResult<AttachmentTypeDTO>>.Ok(
                data: result.Data,
                message: "تم جلب أنواع المرفقات بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, CreateAttachmentTypeDTO dto)
        {
            var command = new UpdateAttachmentTypeCommand(id, dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث نوع المرفق",
                    errorType: "UpdateAttachmentTypeFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم تحديث نوع المرفق بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteAttachmentTypeCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف نوع المرفق",
                    errorType: "DeleteAttachmentTypeFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم حذف نوع المرفق بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
} 