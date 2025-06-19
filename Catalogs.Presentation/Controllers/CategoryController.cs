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
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب الفئات",
                    errorType: "GetAllCategoriesFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<PaginatedResult<CategoryDTO>>.Ok(
                data: result.Data,
                message: "تم جلب الفئات بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetCategoryByIdQuery(id);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب الفئة",
                    errorType: "GetCategoryByIdFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<CategoryDTO>.Ok(
                data: result.Data,
                message: "تم جلب الفئة بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateCategoryDTO dto)
        {
            var command = new CreateCategoryCommand(dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في إنشاء الفئة",
                    errorType: "CreateCategoryFailed",
                    resultStatus: ResultStatus.Failed));

            return CreatedAtAction(nameof(GetById), new { id = result.Data }, Result<Guid>.Ok(
                data: result.Data,
                message: "تم إنشاء الفئة بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CreateCategoryDTO dto)
        {
            var command = new UpdateCategoryCommand(id, dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث الفئة",
                    errorType: "UpdateCategoryFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result.Ok(
                message: "تم تحديث الفئة بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteCategoryCommand(id);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف الفئة",
                    errorType: "DeleteCategoryFailed",
                    resultStatus: ResultStatus.Failed));

            return Ok(Result.Ok(
                message: "تم حذف الفئة بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
} 