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

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<ActionResult<PaginatedResult<BrandDTO>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            GetAllBrandsQuery query = new GetAllBrandsQuery(parameters);
            Result<PaginatedResult<BrandDTO>> result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CreateBrandDTO dto)
        {
            var command = new CreateBrandCommand(dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BrandDTO>> GetById(Guid id)
        {
            var query = new GetBrandByIdQuery(id);
            var result = await _mediator.Send(query);
            
            if (!result.Success)
                return NotFound(result);

            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> Update(Guid id, CreateBrandDTO dto)
        {
            var command = new UpdateBrandCommand(id, dto);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return BadRequest(result);

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var command = new DeleteBrandCommand(id);
            var result = await _mediator.Send(command);
            
            if (!result.Success)
                return BadRequest(result);

            return Ok(result.Data);
        }
    }
} 