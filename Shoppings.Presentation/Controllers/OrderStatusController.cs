using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Entities;
using Core.Pagination;
using Core.Result;
using Shoppings.Application.Commands;
using Shoppings.Application.Commands.DeleteOrderStatus;
using Shoppings.Application.Queries.GetAllOrderStatus;
using Shoppings.Application.Queries.GetOrderStatusById;

namespace Shoppings.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderStatusController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrderStatusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<OrderStatus>> Create([FromBody] CreateOrderStatusDTO dto)
        {
            var command = new CreateOrderStatusCommand(dto.Name);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, [FromBody] CreateOrderStatusDTO dto)
        {
            var command = new UpdateOrderStatusCommand(id, dto.Name);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeleteOrderStatusCommand(id);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<OrderStatus>>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            var query = new GetAllOrderStatusQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<OrderStatus>>> GetById(Guid id)
        {
            var query = new GetOrderStatusByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 