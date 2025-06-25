using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shoppings.Application.Commands.AddToMyCart;
using Shoppings.Application.Queries.GetMyCart;
using Shoppings.Application.DTOs;
using Core.Pagination;
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

        /// <summary>
        /// Get the current user's cart (creates one if it doesn't exist)
        /// </summary>
        [HttpGet("my-cart")]
        public async Task<ActionResult<Result<CartDTO>>> GetMyCart()
        {
            try
            {
                var userId = User.GetId();
                var query = new GetMyCartQuery(userId);
                var result = await _mediator.Send(query);
                
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في جلب سلة التسوق",
                        errorType: "GetMyCartFailed",
                        resultStatus: ResultStatus.Failed));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart for user {UserId}", User.GetId());
                return StatusCode(500, Result<CartDTO>.Fail(
                    message: "فشل في جلب سلة التسوق",
                    errorType: "GetMyCartFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        /// <summary>
        /// Add item to the current user's cart
        /// </summary>
        [HttpPost("add-item")]
        public async Task<ActionResult<Result<Guid>>> AddToMyCart([FromBody] AddToCartDTO dto)
        {
            try
            {
                var userId = User.GetId();
                var command = new AddToMyCartCommand(userId, dto.ItemId, dto.Quantity);
                var result = await _mediator.Send(command);
                
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في إضافة العنصر إلى سلة التسوق",
                        errorType: "AddToMyCartFailed",
                        resultStatus: ResultStatus.Failed));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart for user {UserId}", User.GetId());
                return StatusCode(500, Result<Guid>.Fail(
                    message: "فشل في إضافة العنصر إلى سلة التسوق",
                    errorType: "AddToMyCartFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        /// <summary>
        /// Convert the current user's cart to an order
        /// </summary>
        [HttpPost("checkout")]
        public async Task<ActionResult<Result<Guid>>> Checkout()
        {
            try
            {
                var userId = User.GetId();
                
                // First get the user's cart
                var cartQuery = new GetMyCartQuery(userId);
                var cartResult = await _mediator.Send(cartQuery);
                
                if (!cartResult.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في العثور على سلة التسوق",
                        errorType: "CartNotFound",
                        resultStatus: ResultStatus.Failed));

                // Then convert to order
                var command = new Shoppings.Application.Commands.TransactCartToOrder.TransactCartToOrderCommand(cartResult.Data.Id);
                var result = await _mediator.Send(command);
                
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في تحويل سلة التسوق إلى طلب",
                        errorType: "CheckoutFailed",
                        resultStatus: ResultStatus.Failed));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during checkout for user {UserId}", User.GetId());
                return StatusCode(500, Result<Guid>.Fail(
                    message: "فشل في إتمام عملية الطلب",
                    errorType: "CheckoutFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        /// <summary>
        /// [ADMIN] Get all carts with pagination
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result<PaginatedResult<Cart>>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            try
            {
                var query = new GetAllCartQuery(parameters);
                var result = await _mediator.Send(query);
                
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في جلب سلات التسوق",
                        errorType: "GetCartsFailed",
                        resultStatus: ResultStatus.Failed));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all carts");
                return StatusCode(500, Result<PaginatedResult<Cart>>.Fail(
                    message: "فشل في جلب سلات التسوق",
                    errorType: "GetCartsFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        /// <summary>
        /// Remove item from the current user's cart
        /// </summary>
        [HttpDelete("remove-item/{itemId}")]
        public async Task<ActionResult<Result<bool>>> RemoveFromMyCart(Guid itemId)
        {
            try
            {
                var userId = User.GetId();
                var command = new Shoppings.Application.Commands.RemoveFromMyCart.RemoveFromMyCartCommand(userId, itemId);
                var result = await _mediator.Send(command);
                
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في حذف العنصر من سلة التسوق",
                        errorType: "RemoveFromMyCartFailed",
                        resultStatus: ResultStatus.Failed));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart for user {UserId}", User.GetId());
                return StatusCode(500, Result<bool>.Fail(
                    message: "فشل في حذف العنصر من سلة التسوق",
                    errorType: "RemoveFromMyCartFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        /// <summary>
        /// Update item quantity in the current user's cart
        /// </summary>
        [HttpPut("update-item")]
        public async Task<ActionResult<Result<bool>>> UpdateMyCartItem([FromBody] UpdateCartItemDTO dto)
        {
            try
            {
                var userId = User.GetId();
                var command = new Shoppings.Application.Commands.UpdateMyCartItem.UpdateMyCartItemCommand(userId, dto.ItemId, dto.Quantity);
                var result = await _mediator.Send(command);
                
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في تحديث كمية العنصر في سلة التسوق",
                        errorType: "UpdateMyCartItemFailed",
                        resultStatus: ResultStatus.Failed));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item in cart for user {UserId}", User.GetId());
                return StatusCode(500, Result<bool>.Fail(
                    message: "فشل في تحديث كمية العنصر في سلة التسوق",
                    errorType: "UpdateMyCartItemFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        /// <summary>
        /// Get current user's cart items with pagination and full item details (name, price, etc.)
        /// </summary>
        [HttpGet("my-cart-items")]
        public async Task<ActionResult<Result<PaginatedResult<CartItemWithDetailsDTO>>>> GetMyCartItems([FromQuery] PaginationParameters parameters)
        {
            try
            {
                var userId = User.GetId();
                var query = new Shoppings.Application.Queries.GetMyCartItems.GetMyCartItemsQuery(userId, parameters);
                var result = await _mediator.Send(query);
                
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في جلب عناصر سلة التسوق",
                        errorType: "GetMyCartItemsFailed",
                        resultStatus: ResultStatus.Failed));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart items for user {UserId}", User.GetId());
                return StatusCode(500, Result<PaginatedResult<CartItemWithDetailsDTO>>.Fail(
                    message: "فشل في جلب عناصر سلة التسوق",
                    errorType: "GetMyCartItemsFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }
    }
} 