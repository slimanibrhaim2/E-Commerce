using Core.Pagination;
using Core.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Entities;
using Shoppings.Application.Commands;
using Shoppings.Application.Queries.GetAllOrderActivity;
using Shoppings.Application.Queries.GetOrderActivityById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Shoppings.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderActivityController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrderActivityController> _logger;

        public OrderActivityController(IMediator mediator, ILogger<OrderActivityController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreateOrderActivity([FromBody] CreateOrderActivityDTO dto)
        {
            try
            {
                var command = new CreateOrderActivityCommand( dto.StatusId);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order activity");
                return StatusCode(500, Result<Guid>.Fail(
                    message: "فشل في إنشاء نشاط الطلب",
                    errorType: "CreateOrderActivityFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> UpdateOrderActivity(Guid id, [FromBody] CreateOrderActivityDTO dto)
        {
            try
            {
                var command = new UpdateOrderActivityCommand(id, dto.StatusId);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order activity {OrderActivityId}", id);
                return StatusCode(500, Result<bool>.Fail(
                    message: "فشل في تحديث نشاط الطلب",
                    errorType: "UpdateOrderActivityFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> DeleteOrderActivity(Guid id)
        {
            try
            {
                var command = new DeleteOrderActivityCommand(id);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order activity {OrderActivityId}", id);
                return StatusCode(500, Result<bool>.Fail(
                    message: "فشل في حذف نشاط الطلب",
                    errorType: "DeleteOrderActivityFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<OrderActivity>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new Core.Pagination.PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllOrderActivityQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<OrderActivity>>> GetById(Guid id)
        {
            var query = new GetOrderActivityByIdQuery( id );
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 