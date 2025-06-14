using Microsoft.AspNetCore.Mvc;
using MediatR;
using Catalogs.Application.Commands.AddFeature;
using Catalogs.Application.Commands.UpdateFeature;
using Catalogs.Application.Commands.DeleteFeature;
using Catalogs.Application.DTOs;
using Microsoft.Extensions.Logging;
using Catalogs.Application.Queries.GetFeatureById;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;
using Core.Result;

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FeatureController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<FeatureController> _logger;

        public FeatureController(IMediator mediator, ILogger<FeatureController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("{entityId}")]
        public async Task<ActionResult<Result<Guid>>> AddFeature(Guid entityId, [FromBody] CreateFeatureDTO dto)
        {
            var command = new AddFeatureCommand(entityId, dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في إضافة الميزة",
                    errorType: "AddFeatureFailed",
                    resultStatus: ResultStatus.Failed));

            return CreatedAtAction(nameof(GetById), new { id = result.Data }, Result<Guid>.Ok(
                data: result.Data,
                message: "تم إضافة الميزة بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, [FromBody] CreateFeatureDTO dto)
        {
            var command = new UpdateFeatureCommand(id, dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث الميزة",
                    errorType: "UpdateFeatureFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم تحديث الميزة بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteFeatureCommand(id);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف الميزة",
                    errorType: "DeleteFeatureFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم حذف الميزة بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Result<FeatureDTO>>> GetById(Guid id)
        {
            var query = new GetFeatureByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب الميزة",
                    errorType: "GetFeatureByIdFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result<FeatureDTO>.Ok(
                data: result.Data,
                message: "تم جلب الميزة بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
} 