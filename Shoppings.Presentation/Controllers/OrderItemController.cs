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
using Shoppings.Application.Queries.GetOrderItemById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Shoppings.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderItemController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderItemController> _logger;

        public OrderItemController(IMediator mediator, ILogger<OrderItemController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreateOrderItem([FromBody] CreateOrderItemDTO dto)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order item");
                return StatusCode(500, Result<Guid>.Fail(
                    message: "فشل في إضافة عنصر إلى الطلب",
                    errorType: "CreateOrderItemFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<OrderItem>>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            var query = new GetAllOrderItemQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<OrderItem>>> GetById(Guid id)
        {
            try
            {
                var query = new GetOrderItemByIdQuery(id);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order item {OrderItemId}", id);
                return StatusCode(500, Result<OrderItem>.Fail(
                    message: "فشل في جلب عنصر الطلب",
                    errorType: "GetOrderItemFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> UpdateOrderItem(Guid id, [FromBody] CreateOrderItemDTO dto)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order item {OrderItemId}", id);
                return StatusCode(500, Result<bool>.Fail(
                    message: "فشل في تحديث عنصر الطلب",
                    errorType: "UpdateOrderItemFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> DeleteOrderItem(Guid id)
        {
            try
            {
                var command = new DeleteOrderItemCommand(id);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order item {OrderItemId}", id);
                return StatusCode(500, Result<bool>.Fail(
                    message: "فشل في حذف عنصر من الطلب",
                    errorType: "DeleteOrderItemFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }
    }
} 