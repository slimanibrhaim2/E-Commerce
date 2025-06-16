using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payments.Application.Commands.CreatePaymentStatus;
using Payments.Application.Commands.UpdatePaymentStatus;
using Payments.Application.Commands.DeletePaymentStatus;
using Payments.Application.DTOs;
using Payments.Application.Queries.GetAllPaymentStatuses;
using Payments.Application.Queries.GetPaymentStatusById;
using Payments.Domain.Entities;
using Core.Pagination;
using Core.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Payments.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentStatusController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentStatusController> _logger;

        public PaymentStatusController(IMediator mediator, ILogger<PaymentStatusController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> Create([FromBody] CreatePaymentStatusDTO dto)
        {
            try
            {
                var command = new CreatePaymentStatusCommand(dto.Name);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return BadRequest(result);
                return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment status");
                return StatusCode(500, Result<Guid>.Fail(
                    message: "حدث خطأ أثناء إنشاء حالة الدفع",
                    errorType: "CreatePaymentStatusFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<PaymentStatusDTO>>> GetById(Guid id)
        {
            try
            {
                var query = new GetPaymentStatusByIdQuery(id);
                var result = await _mediator.Send(query);
                if (!result.Success)
                    return NotFound(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment status with ID: {Id}", id);
                return StatusCode(500, Result<PaymentStatusDTO>.Fail(
                    message: "حدث خطأ أثناء استرجاع حالة الدفع",
                    errorType: "GetPaymentStatusFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<PaymentStatusDTO>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
                var query = new GetAllPaymentStatusesQuery(parameters);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all payment statuses");
                return StatusCode(500, Result<PaginatedResult<PaymentStatusDTO>>.Fail(
                    message: "حدث خطأ أثناء استرجاع حالات الدفع",
                    errorType: "GetPaymentStatusesFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, [FromBody] CreatePaymentStatusDTO dto)
        {
            try
            {
                var command = new UpdatePaymentStatusCommand(id, dto.Name);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment status with ID: {Id}", id);
                return StatusCode(500, Result<bool>.Fail(
                    message: "حدث خطأ أثناء تحديث حالة الدفع",
                    errorType: "UpdatePaymentStatusFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            try
            {
                var command = new DeletePaymentStatusCommand(id);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting payment status with ID: {Id}", id);
                return StatusCode(500, Result<bool>.Fail(
                    message: "حدث خطأ أثناء حذف حالة الدفع",
                    errorType: "DeletePaymentStatusFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }
    }
}
