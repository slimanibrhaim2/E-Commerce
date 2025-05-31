using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shoppings.Application.Commands.CreateOrderItem;
using Shoppings.Application.Commands.UpdateOrderItem;
using Shoppings.Application.Commands.DeleteOrderItem;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Entities;
using Core.Result;
using Core.Pagination;
using Shoppings.Application.Queries.GetAllOrderItem;

namespace Shoppings.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrderItemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreateOrderItem([FromBody] CreateOrderItemDTO dto)
        {
            var command = new CreateOrderItemCommand(
                dto.OrderId,
                dto.BaseItemId,
                dto.Quantity,
                dto.Price,
                dto.CouponId
            );
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<OrderItem>>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            var query = new GetAllOrderItemQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItem>> GetById(Guid id)
        {
            // Placeholder: Map to GetOrderItemByIdQuery and send
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> UpdateOrderItem(Guid id, [FromBody] CreateOrderItemDTO dto)
        {
            var command = new UpdateOrderItemCommand(
                id,
                dto.Quantity,
                dto.Price,
                dto.CouponId
            );
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> DeleteOrderItem(Guid id)
        {
            var command = new DeleteOrderItemCommand(id);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
} 