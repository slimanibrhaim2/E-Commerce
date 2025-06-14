using Microsoft.AspNetCore.Mvc;
using MediatR;
using Catalogs.Application.Queries.GetAllBrands;
using Catalogs.Application.Commands.CreateBrand;
using Catalogs.Application.Commands.UpdateBrand;
using Catalogs.Application.Commands.DeleteBrand;
using Catalogs.Application.DTOs;
using Microsoft.Extensions.Logging;
using Catalogs.Application.Queries.GetBrandById;
using Core.Pagination;
using Core.Result;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BrandController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BrandController> _logger;

        public BrandController(IMediator mediator, ILogger<BrandController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<Result<PaginatedResult<BrandDTO>>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            GetAllBrandsQuery query = new GetAllBrandsQuery(parameters);
            Result<PaginatedResult<BrandDTO>> result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب العلامات التجارية",
                    errorType: "GetAllBrandsFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<PaginatedResult<BrandDTO>>.Ok(
                data: result.Data,
                message: "تم جلب العلامات التجارية بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> Create(CreateBrandDTO dto)
        {
            var command = new CreateBrandCommand(dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في إنشاء العلامة التجارية",
                    errorType: "CreateBrandFailed",
                    resultStatus: ResultStatus.Failed));

            return CreatedAtAction(nameof(GetById), new { id = result.Data }, Result<Guid>.Ok(
                data: result.Data,
                message: "تم إنشاء العلامة التجارية بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Result<BrandDTO>>> GetById(Guid id)
        {
            var query = new GetBrandByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب العلامة التجارية",
                    errorType: "GetBrandByIdFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result<BrandDTO>.Ok(
                data: result.Data,
                message: "تم جلب العلامة التجارية بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, CreateBrandDTO dto)
        {
            var command = new UpdateBrandCommand(id, dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث العلامة التجارية",
                    errorType: "UpdateBrandFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم تحديث العلامة التجارية بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteBrandCommand(id);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف العلامة التجارية",
                    errorType: "DeleteBrandFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result<bool>.Ok(
                data: result.Data,
                message: "تم حذف العلامة التجارية بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
} 