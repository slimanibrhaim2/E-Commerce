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

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<ActionResult<PaginatedResult<CategoryDTO>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            var query = new GetAllCategoriesQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody]CreatCategoryDTO dto)
        {
            var command = new CreateCategoryCommand(dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetById(Guid id)
        {
            var query = new GetCategoryByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return NotFound(result);

            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, CreatCategoryDTO dto)
        {
            var command = new UpdateCategoryCommand(id, dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return BadRequest(result);

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var command = new DeleteCategoryCommand(id);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return BadRequest(result);

            return Ok(result.Data);
        }
    }
} 