using Microsoft.AspNetCore.Mvc;
using MediatR;
using Catalogs.Application.Commands.AddMedia;
using Catalogs.Application.Commands.DeleteMedia;
using Catalogs.Application.DTOs;
using Microsoft.Extensions.Logging;
using Catalogs.Application.Queries.GetMediaById;
using Catalogs.Application.Commands.UpdateMedia;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;
using Core.Result;

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MediaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MediaController> _logger;

        public MediaController(IMediator mediator, ILogger<MediaController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("{itemId}")]
        public async Task<ActionResult<Result<Guid>>> AddMedia(Guid itemId, [FromBody] CreateMediaDTO dto)
        {
            var command = new AddMediaCommand(itemId, dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في إضافة الوسائط",
                    errorType: "AddMediaFailed",
                    resultStatus: ResultStatus.Failed));

            return CreatedAtAction(nameof(GetById), new { id = result.Data }, Result<Guid>.Ok(
                data: result.Data,
                message: "تم إضافة الوسائط بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, [FromBody] CreateMediaDTO dto)
        {
            var command = new UpdateMediaCommand(id, dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث الوسائط",
                    errorType: "UpdateMediaFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم تحديث الوسائط بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteMediaCommand(id);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف الوسائط",
                    errorType: "DeleteMediaFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم حذف الوسائط بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Result<MediaDTO>>> GetById(Guid id)
        {
            var query = new GetMediaByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب الوسائط",
                    errorType: "GetMediaFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result<MediaDTO>.Ok(
                data: result.Data,
                message: "تم جلب الوسائط بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
} 