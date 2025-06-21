using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shoppings.Application.Commands.CreateCartItem;
using Shoppings.Application.Commands.UpdateCartItem;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Entities;
using Core.Pagination;
using Core.Result;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;
using Microsoft.Extensions.Logging;
using Shoppings.Application.Commands;
using Shoppings.Application.Queries.GetAllCartItem;

namespace Shoppings.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartItemController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CartItemController> _logger;

        public CartItemController(IMediator mediator, ILogger<CartItemController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreateCartItem([FromBody] CreateCartItemDTO dto)
        {
            try
            {
                var command = new CreateCartItemCommand(dto.CartId, dto.ItemId, dto.Quantity);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cart item for user {UserId}", User.GetId());
                return StatusCode(500, Result<Guid>.Fail(
                    message: "فشل في إضافة عنصر إلى سلة التسوق",
                    errorType: "CreateCartItemFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> UpdateCartItem(Guid id, [FromBody] CreateCartItemDTO dto)
        {
            try
            {
                var command = new UpdateCartItemCommand(id, dto.Quantity);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item {CartItemId} for user {UserId}", id, User.GetId());
                return StatusCode(500, Result<bool>.Fail(
                    message: "فشل في تحديث عنصر سلة التسوق",
                    errorType: "UpdateCartItemFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> DeleteCartItem(Guid id)
        {
            try
            {
                var command = new DeleteCartItemCommand(id);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cart item {CartItemId} for user {UserId}", id, User.GetId());
                return StatusCode(500, Result<bool>.Fail(
                    message: "فشل في حذف عنصر من سلة التسوق",
                    errorType: "DeleteCartItemFailed",
                    resultStatus: ResultStatus.Failed));
            }
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