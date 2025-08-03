using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shoppings.Application.Commands.CreateOrder;
using Shoppings.Application.Commands.UpdateOrder;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Entities;
using Core.Pagination;
using Core.Result;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;
using Microsoft.Extensions.Logging;
using Shoppings.Application.Commands;
using Shoppings.Application.Queries.GetAllOrder;
using Shoppings.Application.Queries.GetOrderById;
using Shoppings.Application.Queries.GetMyOrders;
using Shoppings.Application.Commands.CancelOrder;
using Shoppings.Application.Commands.MarkOrderDelivered;
using Shoppings.Application.Queries.GetOrdersForSeller;
using Shoppings.Application.Commands.PayOrder;
using Shoppings.Application.Commands.Checkout;
using Shoppings.Application.Queries.GetMyCart;
using Shoppings.Application.Queries.GetOrderBlockchainDetails;
using Shared.Contracts.Commands;
using Shared.Contracts.DTOs.Blockchain;
using Shared.Contracts.Queries;

namespace Shoppings.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IMediator mediator, ILogger<OrderController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreateOrder([FromBody] CreateOrderDTO dto)
        {
            try
            {
                var userId = User.GetId();
                var command = new CreateOrderCommand( userId,dto.OrderActivityId);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for user {UserId}", User.GetId());
                return StatusCode(500, Result<Guid>.Fail(
                    message: "فشل في إنشاء الطلب",
                    errorType: "CreateOrderFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> UpdateOrder(Guid id, [FromBody] Guid orderActivity)
        {
            try
            {
                var userId = User.GetId();
                var command = new UpdateOrderCommand(id, orderActivity);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId} for user {UserId}", id, User.GetId());
                return StatusCode(500, Result<bool>.Fail(
                    message: "فشل في تحديث الطلب",
                    errorType: "UpdateOrderFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> DeleteOrder(Guid id)
        {
            try
            {
                var userId = User.GetId();
                var command = new DeleteOrderCommand(id);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order {OrderId} for user {UserId}", id, User.GetId());
                return StatusCode(500, Result<bool>.Fail(
                    message: "فشل في حذف الطلب",
                    errorType: "DeleteOrderFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<Result<bool>>> CancelOrder(Guid id)
        {
            try
            {
                var userId = User.GetId();
                var command = new CancelOrderCommand(id, userId);
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
                _logger.LogError(ex, "Error cancelling order {OrderId} for user {UserId}", id, User.GetId());
                return StatusCode(500, Result<bool>.Fail(
                    message: "فشل في إلغاء الطلب",
                    errorType: "CancelOrderFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPost("{id}/mark-delivered")]
        public async Task<ActionResult<Result<bool>>> MarkOrderDelivered(Guid id)
        {
            try
            {
                var userId = User.GetId();
                var command = new MarkOrderDeliveredCommand(id, userId);
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
                _logger.LogError(ex, "Error marking order {OrderId} as delivered for user {UserId}", id, User.GetId());
                return StatusCode(500, Result<bool>.Fail(
                    message: "فشل في تحديث حالة الطلب إلى تم التوصيل",
                    errorType: "MarkDeliveredFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Result<PaginatedResult<MyOrderDTO>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllOrderQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result.Success ? 
                Result<PaginatedResult<MyOrderDTO>>.Ok(
                    data: result.Data,
                    message: "تم جلب جميع الطلبات بنجاح (مخصص للمدراء فقط)",
                    resultStatus: ResultStatus.Success) : result);
        }

        [HttpGet("my-orders")]
        public async Task<ActionResult<Result<PaginatedResult<MyOrderDTO>>>> GetMyOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = User.GetId();
                var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
                var query = new GetMyOrdersQuery(userId, parameters);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for authenticated user");
                return StatusCode(500, Result<PaginatedResult<MyOrderDTO>>.Fail(
                    message: "فشل في جلب الطلبات",
                    errorType: "GetOrdersFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet("by-user/{userId}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Result<PaginatedResult<MyOrderDTO>>>> GetOrdersByUserId(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
                var query = new GetMyOrdersQuery(userId, parameters); // Using the same query, just with different userId
                var result = await _mediator.Send(query);
                return Ok(result.Success ? 
                    Result<PaginatedResult<MyOrderDTO>>.Ok(
                        data: result.Data,
                        message: "تم جلب طلبات المستخدم بنجاح (مخصص للمدراء فقط)",
                        resultStatus: ResultStatus.Success) : result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for user {UserId}", userId);
                return StatusCode(500, Result<PaginatedResult<MyOrderDTO>>.Fail(
                    message: "فشل في جلب طلبات المستخدم",
                    errorType: "GetOrdersByUserIdFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<OrderWithItemsDTO>>> GetById(Guid id)
        {
            try
            {
                var query = new GetOrderByIdQuery(id);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order {OrderId}", id);
                return StatusCode(500, Result<OrderWithItemsDTO>.Fail(
                    message: "فشل في جلب الطلب",
                    errorType: "GetOrderFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet("{id}/blockchain")]
        public async Task<ActionResult<Result<OrderBlockchainDTO>>> GetOrderBlockchainDetails(Guid id)
        {
            try
            {
                var query = new GetOrderBlockchainDetailsQuery(id);
                var result = await _mediator.Send(query);
                
                if (!result.Success)
                {
                    return result.ResultStatus switch
                    {
                        ResultStatus.NotFound => NotFound(result),
                        ResultStatus.ValidationError => BadRequest(result),
                        _ => StatusCode(500, result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting blockchain details for order {OrderId}", id);
                return StatusCode(500, Result<OrderBlockchainDTO>.Fail(
                    message: "فشل في جلب تفاصيل البلوك تشين للطلب",
                    errorType: "GetOrderBlockchainDetailsFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet("seller-orders")]
        public async Task<ActionResult<Result<PaginatedResult<SellerOrderDTO>>>> GetOrdersForSeller([FromQuery] PaginationParameters parameters)
        {
            try
            {
                var sellerId = User.GetId();
                var query = new GetOrdersForSellerQuery(sellerId, parameters);
                var result = await _mediator.Send(query);

                if (!result.Success)
                {
                    return result.ResultStatus switch
                    {
                        ResultStatus.NotFound => NotFound(result),
                        ResultStatus.ValidationError => BadRequest(result),
                        _ => StatusCode(500, result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for seller");
                return StatusCode(500, Result<PaginatedResult<SellerOrderDTO>>.Fail(
                    message: "حدث خطأ أثناء جلب الطلبات",
                    errorType: "GetSellerOrdersFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }
        /// <summary>
        /// Convert the current user's cart to an order
        /// </summary>
        [HttpPost("Checkout")]
        public async Task<ActionResult<Result<OrderWithItemsDTO>>> Checkout([FromBody] Guid AddressId)
        {
            try
            {
                var userId = User.GetId();

                // Get user's cart
                var cartQuery = new GetMyCartQuery(userId);
                var cartResult = await _mediator.Send(cartQuery);

                if (!cartResult.Success)
                {
                    return StatusCode(500, Result<OrderWithItemsDTO>.Fail(
                        message: cartResult.Message,
                        errorType: cartResult.ErrorType,
                        resultStatus: cartResult.ResultStatus));
                }

                // Create checkout command
                var command = new CheckoutCommand(cartResult.Data.Id, AddressId);
                var checkoutResult = await _mediator.Send(command);

                if (!checkoutResult.Success)
                {
                    return StatusCode(500, Result<OrderWithItemsDTO>.Fail(
                        message: checkoutResult.Message,
                        errorType: checkoutResult.ErrorType,
                        resultStatus: checkoutResult.ResultStatus));
                }

                // Get the created order details using GetOrderById
                var orderQuery = new GetOrderByIdQuery(checkoutResult.Data);
                var orderResult = await _mediator.Send(orderQuery);

                return Ok(orderResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during checkout for user {UserId}", User.GetId());
                return StatusCode(500, Result<OrderWithItemsDTO>.Fail(
                    message: "فشل في إتمام عملية الشراء",
                    errorType: "CheckoutFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }
        [HttpPost("{id}/pay")]
        public async Task<ActionResult<Result>> PayOrder(Guid id)
        {
            try
            {
                var command = new PayOrderCommand(id);
                var result = await _mediator.Send(command);

                if (!result.Success)
                {
                    return result.ResultStatus switch
                    {
                        ResultStatus.NotFound => NotFound(result),
                        ResultStatus.ValidationError => BadRequest(result),
                        _ => StatusCode(500, result)
                    };
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error paying order {OrderId}", id);
                return StatusCode(500, Result.Fail(
                    message: "حدث خطأ أثناء دفع الطلب",
                    errorType: "PayOrderFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }
    }
} 