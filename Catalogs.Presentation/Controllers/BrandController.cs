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
        public async Task<IActionResult> GetAll([FromQuery] PaginationParameters parameters)
        {
            GetAllBrandsQuery query = new GetAllBrandsQuery(parameters);
            Result<PaginatedResult<BrandDTO>> result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, new
                {
                    resultStatus = (int)ResultStatus.Failed,
                    success = false,
                    message = "فشل في جلب العلامات التجارية",
                    errorType = "GetAllBrandsFailed"
                });
            return Ok(new
            {
                resultStatus = (int)ResultStatus.Success,
                success = true,
                message = "تم جلب العلامات التجارية بنجاح",
                errorType = (string)null,
                data = result.Data.Data,
                pagination = new
                {
                    pageNumber = result.Data.PageNumber,
                    pageSize = result.Data.PageSize,
                    totalPages = result.Data.TotalPages,
                    totalCount = result.Data.TotalCount,
                    hasPreviousPage = result.Data.HasPreviousPage,
                    hasNextPage = result.Data.HasNextPage
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBrandDTO dto)
        {
            var command = new CreateBrandCommand(dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, new
                {
                    resultStatus = (int)ResultStatus.Failed,
                    success = false,
                    message = "فشل في إنشاء العلامة التجارية",
                    errorType = "CreateBrandFailed"
                });

            return CreatedAtAction(nameof(GetById), new { id = result.Data }, new
            {
                resultStatus = (int)ResultStatus.Success,
                success = true,
                message = "تم إنشاء العلامة التجارية بنجاح",
                errorType = (string)null,
                data = result.Data
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetBrandByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return StatusCode(500, new
                {
                    resultStatus = (int)ResultStatus.Failed,
                    success = false,
                    message = "فشل في جلب العلامة التجارية",
                    errorType = "GetBrandByIdFailed"
                });

            return Ok(new
            {
                resultStatus = (int)ResultStatus.Success,
                success = true,
                message = "تم جلب العلامة التجارية بنجاح",
                errorType = (string)null,
                data = result.Data
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CreateBrandDTO dto)
        {
            var command = new UpdateBrandCommand(id, dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, new
                {
                    resultStatus = (int)ResultStatus.Failed,
                    success = false,
                    message = "فشل في تحديث العلامة التجارية",
                    errorType = "UpdateBrandFailed"
                });

            return Ok(new
            {
                resultStatus = (int)ResultStatus.Success,
                success = true,
                message = "تم تحديث العلامة التجارية بنجاح",
                errorType = (string)null,
                data = result.Data
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteBrandCommand(id);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, new
                {
                    resultStatus = (int)ResultStatus.Failed,
                    success = false,
                    message = "فشل في حذف العلامة التجارية",
                    errorType = "DeleteBrandFailed"
                });

            return Ok(new
            {
                resultStatus = (int)ResultStatus.Success,
                success = true,
                message = "تم حذف العلامة التجارية بنجاح",
                errorType = (string)null,
                data = result.Data
            });
        }
    }
} 