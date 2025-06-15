using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Entities;
using Core.Pagination;
using Core.Result;
using Shoppings.Application.Commands;
using Shoppings.Application.Commands.DeleteOrderStatus;
using Shoppings.Application.Queries.GetAllOrderStatus;
using Shoppings.Application.Queries.GetOrderStatusById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Shoppings.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderStatusController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderStatusController> _logger;

        public OrderStatusController(IMediator mediator, ILogger<OrderStatusController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreateOrderStatus([FromBody] CreateOrderStatusDTO dto)
        {
            try
            {
                var command = new CreateOrderStatusCommand(dto.Name);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order status");
                return StatusCode(500, Result<Guid>.Fail(
                    message: "فشل في إنشاء حالة الطلب",
                    errorType: "CreateOrderStatusFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> UpdateOrderStatus(Guid id, [FromBody] CreateOrderStatusDTO dto)
        {
            try
            {
                var command = new UpdateOrderStatusCommand(id, dto.Name);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status {OrderStatusId}", id);
                return StatusCode(500, Result<bool>.Fail(
                    message: "فشل في تحديث حالة الطلب",
                    errorType: "UpdateOrderStatusFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> DeleteOrderStatus(Guid id)
        {
            try
            {
                var command = new DeleteOrderStatusCommand(id);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order status {OrderStatusId}", id);
                return StatusCode(500, Result<bool>.Fail(
                    message: "فشل في حذف حالة الطلب",
                    errorType: "DeleteOrderStatusFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<OrderStatus>>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            var query = new GetAllOrderStatusQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<OrderStatus>>> GetById(Guid id)
        {
            var query = new GetOrderStatusByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 