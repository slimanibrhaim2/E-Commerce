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
using Shoppings.Application.Commands.TransactCartToOrder;

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
        /// Add item to the current user's cart. If item already exists, increases its quantity.
        /// </summary>
        [HttpPost("add-item")]
        public async Task<ActionResult<Result>> AddToMyCart([FromBody] AddToCartDTO dto)
        {
            try
            {
                // Validate input
                if (dto == null)
                {
                    return BadRequest(Result.Fail(
                        message: "البيانات المطلوبة غير موجودة",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError));
                }

                if (dto.ItemId == Guid.Empty)
                {
                    return BadRequest(Result.Fail(
                        message: "معرف العنصر غير صالح",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError));
                }

                if (dto.Quantity <= 0)
                {
                    return BadRequest(Result.Fail(
                        message: "الكمية يجب أن تكون أكبر من صفر",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError));
                }

                var userId = User.GetId();
                var command = new AddToMyCartCommand(userId, dto.ItemId, dto.Quantity);
                var result = await _mediator.Send(command);
                
                if (!result.Success)
                {
                    // Map different error types to appropriate HTTP status codes
                    return result.ResultStatus switch
                    {
                        ResultStatus.ValidationError => BadRequest(result),
                        ResultStatus.NotFound => NotFound(result),
                        _ => StatusCode(500, result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding/updating item in cart for user {UserId}", User.GetId());
                return StatusCode(500, Result.Fail(
                    message: "فشل في إضافة/تحديث العنصر في سلة التسوق",
                    errorType: "AddToMyCartFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        /// <summary>
        /// Convert the current user's cart to an order
        /// </summary>
        [HttpPost("TransactCartToOrder")]
        public async Task<ActionResult<Result<Guid>>> TransactCartToOrder([FromBody] Guid AddressId)
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
                var command = new TransactCartToOrderCommand(cartResult.Data.Id, AddressId);
                var result = await _mediator.Send(command);
                
                if (!result.Success)
                    return StatusCode(500, Result.Fail(
                        message: "فشل في تحويل سلة التسوق إلى طلب",
                        errorType: "TransactCartToOrderFailed",
                        resultStatus: ResultStatus.Failed));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Transact Cart ToOrder for user {UserId}", User.GetId());
                return StatusCode(500, Result<Guid>.Fail(
                    message: "فشل في إتمام عملية الطلب",
                    errorType: "TransactCartToOrderFailed",
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
        public async Task<ActionResult<Result>> RemoveFromMyCart(Guid itemId)
        {
            try
            {
                if (itemId == Guid.Empty)
                {
                    return BadRequest(Result.Fail(
                        message: "معرف العنصر غير صالح",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError));
                }

                var userId = User.GetId();
                var command = new Shoppings.Application.Commands.RemoveFromMyCart.RemoveFromMyCartCommand(userId, itemId);
                var result = await _mediator.Send(command);
                
                if (!result.Success)
                {
                    // Map different error types to appropriate HTTP status codes
                    return result.ResultStatus switch
                    {
                        ResultStatus.ValidationError => BadRequest(result),
                        ResultStatus.NotFound => NotFound(result),
                        _ => StatusCode(500, result)
                    };
                }

                return Ok(Result.Ok(
                    message: "تم حذف العنصر من سلة التسوق بنجاح",
                    resultStatus: ResultStatus.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item {ItemId} from cart for user {UserId}", itemId, User.GetId());
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف العنصر من سلة التسوق",
                    errorType: "RemoveFromCartFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex));
            }
        }

        /// <summary>
        /// Update item quantity in the current user's cart
        /// </summary>
        [HttpPut("update-item")]
        public async Task<ActionResult<Result>> UpdateMyCartItem([FromBody] UpdateCartItemDTO dto, [FromQuery] PaginationParameters parameters)
        {
            try
            {
                // Validate input
                if (dto == null)
                {
                    return BadRequest(Result.Fail(
                        message: "البيانات المطلوبة غير موجودة",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError));
                }

                if (dto.ItemId == Guid.Empty)
                {
                    return BadRequest(Result.Fail(
                        message: "معرف العنصر غير صالح",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError));
                }

                if (dto.Quantity < 0)
                {
                    return BadRequest(Result.Fail(
                        message: "الكمية يجب أن تكون أكبر من أو تساوي صفر",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError));
                }

                var userId = User.GetId();

                // If quantity is 0, remove the item
                if (dto.Quantity == 0)
                {
                    return await RemoveFromMyCart(dto.ItemId);
                }

                // Set default pagination parameters if not provided
                if (parameters == null)
                {
                    parameters = new PaginationParameters { PageNumber = 1, PageSize = 10 };
                }

                // Update quantity
                var command = new Shoppings.Application.Commands.UpdateMyCartItem.UpdateMyCartItemCommand(
                    userId, 
                    dto.ItemId, 
                    dto.Quantity,
                    parameters);
                    
                var result = await _mediator.Send(command);
                
                if (!result.Success)
                {
                    // Map different error types to appropriate HTTP status codes
                    return result.ResultStatus switch
                    {
                        ResultStatus.ValidationError => BadRequest(result),
                        ResultStatus.NotFound => NotFound(result),
                        _ => StatusCode(500, result)
                    };
                }

                return Ok(Result.Ok(
                    message: "تم تحديث كمية العنصر في سلة التسوق بنجاح",
                    resultStatus: ResultStatus.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item in cart for user {UserId}", User.GetId());
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث كمية العنصر في سلة التسوق",
                    errorType: "UpdateMyCartItemFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex));
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