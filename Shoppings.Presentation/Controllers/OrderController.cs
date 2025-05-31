using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shoppings.Application.Commands.CreateOrder;
using Shoppings.Application.Commands.UpdateOrder;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Entities;
using Core.Result;
using Core.Pagination;
using Shoppings.Application.Queries;
using Shoppings.Application.Queries.GetAllOrder;
using Shoppings.Application.Commands;

namespace Shoppings.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreateOrder([FromBody] CreateOrderDTO dto)
        {
            var command = new CreateOrderCommand(dto.UserId, dto.OrderActivityId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> UpdateOrder(Guid id, [FromBody] CreateOrderDTO dto)
        {
            var command = new UpdateOrderCommand(id, dto.OrderActivityId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> DeleteOrder(Guid id)
        {
            var command = new DeleteOrderCommand(id);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<Order>>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            var query = new GetAllOrderQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(Guid id)
        {
            // Placeholder: Map to GetOrderByIdQuery and send
            return Ok();
        }
    }
} 