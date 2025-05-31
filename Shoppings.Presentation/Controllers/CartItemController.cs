using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shoppings.Application.Commands.CreateCartItem;
using Shoppings.Application.Commands.UpdateCartItem;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Entities;
using Core.Pagination;
using Core.Result;
using Shoppings.Application.Queries.GetAllCartItem;
using Shoppings.Application.Commands;

namespace Shoppings.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartItemController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CartItemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreateCartItem([FromBody] CreateCartItemDTO dto)
        {
            var command = new CreateCartItemCommand(dto.CartId, dto.BaseItemId, dto.Quantity);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> UpdateCartItem(Guid id, [FromBody] CreateCartItemDTO dto)
        {
            var command = new UpdateCartItemCommand(id, dto.Quantity);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> DeleteCartItem(Guid id)
        {
            var command = new DeleteCartItemCommand(id);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<CartItem>>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            var query = new GetAllCartItemQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CartItem>> GetById(Guid id)
        {
            // Placeholder: Map to GetCartItemByIdQuery and send
            return Ok();
        }
    }
} 