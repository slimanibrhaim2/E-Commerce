using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shoppings.Application.Commands.CreateCart;
using Shoppings.Application.Commands.UpdateCart;
using Shoppings.Application.Commands.DeleteCart;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Entities;
using Core.Pagination;
using Core.Result;
using Shoppings.Application.Queries.GetAllCart;

namespace Shoppings.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreateCart([FromBody] CreateCartDTO dto)
        {
            var command = new CreateCartCommand(dto.UserId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> UpdateCart(Guid id, [FromBody] CreateCartDTO dto)
        {
            var command = new UpdateCartCommand(id, dto.UserId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> DeleteCart(Guid id)
        {
            var command = new DeleteCartCommand(id);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<Cart>>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            var query = new GetAllCartQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetById(Guid id)
        {
            // Placeholder: Map to GetCartByIdQuery and send
            return Ok();
        }

        [HttpPost("{id}/transact-to-order")]
        public async Task<ActionResult<Result<Guid>>> TransactToOrder(Guid id)
        {
            var command = new Shoppings.Application.Commands.TransactCartToOrder.TransactCartToOrderCommand(id);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
} 