using Microsoft.AspNetCore.Mvc;
using MediatR;
using Catalogs.Application.Queries.GetAllCategories;
using Catalogs.Application.Commands.CreateCategory;
using Catalogs.Application.Commands.UpdateCategory;
using Catalogs.Application.Commands.DeleteCategory;
using Catalogs.Application.DTOs;
using Microsoft.Extensions.Logging;
using Catalogs.Application.Queries.GetCategoryById;
using Core.Pagination;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;
using Core.Result;

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(IMediator mediator, ILogger<CategoryController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParameters parameters)
        {
            var query = new GetAllCategoriesQuery(parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, new
                {
                    resultStatus = (int)ResultStatus.Failed,
                    success = false,
                    message = "فشل في جلب الفئات",
                    errorType = "GetAllCategoriesFailed"
                });
            return Ok(new
            {
                resultStatus = (int)ResultStatus.Success,
                success = true,
                message = "تم جلب الفئات بنجاح",
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

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetCategoryByIdQuery(id);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, new
                {
                    resultStatus = (int)ResultStatus.Failed,
                    success = false,
                    message = "فشل في جلب الفئة",
                    errorType = "GetCategoryByIdFailed"
                });
            return Ok(new
            {
                resultStatus = (int)ResultStatus.Success,
                success = true,
                message = "تم جلب الفئة بنجاح",
                errorType = (string)null,
                data = result.Data
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateCategoryDTO dto)
        {
            var command = new CreateCategoryCommand(dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, new
                {
                    resultStatus = (int)ResultStatus.Failed,
                    success = false,
                    message = "فشل في إنشاء الفئة",
                    errorType = "CreateCategoryFailed"
                });

            return CreatedAtAction(nameof(GetById), new { id = result.Data }, new
            {
                resultStatus = (int)ResultStatus.Success,
                success = true,
                message = "تم إنشاء الفئة بنجاح",
                errorType = (string)null,
                data = result.Data
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CreateCategoryDTO dto)
        {
            var command = new UpdateCategoryCommand(id, dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, new
                {
                    resultStatus = (int)ResultStatus.Failed,
                    success = false,
                    message = "فشل في تحديث الفئة",
                    errorType = "UpdateCategoryFailed"
                });

            return Ok(new
            {
                resultStatus = (int)ResultStatus.Success,
                success = true,
                message = "تم تحديث الفئة بنجاح",
                errorType = (string)null,
                data = result.Data
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteCategoryCommand(id);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, new
                {
                    resultStatus = (int)ResultStatus.Failed,
                    success = false,
                    message = "فشل في حذف الفئة",
                    errorType = "DeleteCategoryFailed"
                });

            return Ok(new
            {
                resultStatus = (int)ResultStatus.Success,
                success = true,
                message = "تم حذف الفئة بنجاح",
                errorType = (string)null,
                data = result.Data
            });
        }
    }
} 