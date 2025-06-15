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

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<Order>>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            var query = new GetAllOrderQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(Guid id)
        {
            // Placeholder: Map to GetOrderByIdQuery and send
            return Ok();
        }
    }
} 