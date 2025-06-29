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
using Shoppings.Application.Commands.TransactCartToOrder;
using Shoppings.Application.Commands.CancelOrder;
using Shoppings.Application.Commands.MarkOrderDelivered;

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
        public async Task<ActionResult<Result<PaginatedResult<Order>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllOrderQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
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
    }
} 