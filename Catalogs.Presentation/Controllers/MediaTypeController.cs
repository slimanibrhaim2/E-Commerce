using Microsoft.AspNetCore.Mvc;
using MediatR;
using Catalogs.Application.DTOs;
using Catalogs.Application.Commands.CreateMediaType;
using Catalogs.Application.Commands.UpdateMediaType;
using Catalogs.Application.Commands.DeleteMediaType;
using Microsoft.Extensions.Logging;
using Catalogs.Application.Queries.GetAllMediaTypes;
using Catalogs.Application.Queries.GetMediaTypeById;
using Core.Pagination;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;
using Core.Result;

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MediaTypeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MediaTypeController> _logger;

        public MediaTypeController(IMediator mediator, ILogger<MediaTypeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<Result<PaginatedResult<MediaTypeDTO>>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            var query = new GetAllMediaTypesQuery(parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب أنواع الوسائط",
                    errorType: "GetAllMediaTypesFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<PaginatedResult<MediaTypeDTO>>.Ok(
                data: result.Data,
                message: "تم جلب أنواع الوسائط بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Result<MediaTypeDTO>>> GetById(Guid id)
        {
            var query = new GetMediaTypeByIdQuery(id);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب نوع الوسائط",
                    errorType: "GetMediaTypeByIdFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<MediaTypeDTO>.Ok(
                data: result.Data,
                message: "تم جلب نوع الوسائط بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Result<Guid>>> CreateMediaType([FromBody] CreateMediaTypeDTO dto)
        {
            var command = new CreateMediaTypeCommand(dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في إنشاء نوع الوسائط",
                    errorType: "CreateMediaTypeFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<Guid>.Ok(
                data: result.Data,
                message: "تم إنشاء نوع الوسائط بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Result<bool>>> UpdateMediaType(Guid id, [FromBody] CreateMediaTypeDTO dto)
        {
            var command = new UpdateMediaTypeCommand(id, dto);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث نوع الوسائط",
                    errorType: "UpdateMediaTypeFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم تحديث نوع الوسائط بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Result<bool>>> DeleteMediaType(Guid id)
        {
            var command = new DeleteMediaTypeCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف نوع الوسائط",
                    errorType: "DeleteMediaTypeFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم حذف نوع الوسائط بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
} 