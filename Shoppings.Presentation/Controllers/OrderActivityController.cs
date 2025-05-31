using Core.Pagination;
using Core.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Entities;
using Shoppings.Application.Commands;
using Shoppings.Application.Queries.GetAllOrderActivity;
using Shoppings.Application.Queries.GetOrderActivityById;

namespace Shoppings.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderActivityController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrderActivityController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<OrderActivity>> Create([FromBody] CreateOrderActivityDTO dto)
        {
            var command = new CreateOrderActivityCommand(dto.StatusId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, [FromBody] CreateOrderActivityDTO dto)
        {
            var command = new UpdateOrderActivityCommand (id, dto.StatusId );
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteOrderActivityCommand ( id );
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<OrderActivity>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new Core.Pagination.PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllOrderActivityQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<OrderActivity>>> GetById(Guid id)
        {
            var query = new GetOrderActivityByIdQuery( id );
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 