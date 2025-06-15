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
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;
using Microsoft.Extensions.Logging;

namespace Shoppings.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CartController> _logger;

        public CartController(IMediator mediator, ILogger<CartController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreateCart()
        {
            try
            {
                var userId = User.GetId();
                var command = new CreateCartCommand(userId);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cart for user {UserId}", User.GetId());
                return StatusCode(500, Result<Guid>.Fail(
                    message: "فشل في إنشاء سلة التسوق",
                    errorType: "CreateCartFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> UpdateCart(Guid id)
        {
            try
            {
                var userId = User.GetId();
                var command = new UpdateCartCommand(id, userId);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart {CartId} for user {UserId}", id, User.GetId());
                return StatusCode(500, Result<bool>.Fail(
                    message: "فشل في تحديث سلة التسوق",
                    errorType: "UpdateCartFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> DeleteCart(Guid id)
        {
            try
            {
                var command = new DeleteCartCommand(id);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cart {CartId} for user {UserId}", id, User.GetId());
                return StatusCode(500, Result<bool>.Fail(
                    message: "فشل في حذف سلة التسوق",
                    errorType: "DeleteCartFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<Cart>>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            try
            {
                var query = new GetAllCartQuery(parameters);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting carts for user {UserId}", User.GetId());
                return StatusCode(500, Result<PaginatedResult<Cart>>.Fail(
                    message: "فشل في جلب سلات التسوق",
                    errorType: "GetCartsFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetById(Guid id)
        {
            try
            {
                var userId = User.GetId();
                // TODO: Implement GetCartByIdQuery with user authorization
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart {CartId} for user {UserId}", id, User.GetId());
                return StatusCode(500, Result<Cart>.Fail(
                    message: "فشل في جلب سلة التسوق",
                    errorType: "GetCartFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPost("{id}/transact-to-order")]
        public async Task<ActionResult<Result<Guid>>> TransactToOrder(Guid id)
        {
            try
            {
                var command = new Shoppings.Application.Commands.TransactCartToOrder.TransactCartToOrderCommand(id);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transacting cart {CartId} to order for user {UserId}", id, User.GetId());
                return StatusCode(500, Result<Guid>.Fail(
                    message: "فشل في تحويل سلة التسوق إلى طلب",
                    errorType: "TransactCartFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }
    }
} 