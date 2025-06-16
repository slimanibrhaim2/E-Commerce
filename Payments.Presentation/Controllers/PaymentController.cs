using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payments.Application.Commands.CreatePayment;
using Payments.Application.Commands.UpdatePayment;
using Payments.Application.Commands.DeletePayment;
using Payments.Application.DTOs;
using Payments.Application.Queries.GetAllPayments;
using Payments.Application.Queries.GetPaymentById;
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
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IMediator mediator, ILogger<PaymentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> Create([FromBody] CreatePaymentDTO dto)
        {
            try
            {
                var command = new CreatePaymentCommand(dto.OrderId, dto.Amount, dto.PaymentMethodId, dto.StatusId);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return BadRequest(result);
                return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment");
                return StatusCode(500, Result<Guid>.Fail(
                    message: "حدث خطأ أثناء إنشاء الدفع",
                    errorType: "CreatePaymentFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<PaymentDTO>>> GetById(Guid id)
        {
            try
            {
                var query = new GetPaymentByIdQuery(id);
                var result = await _mediator.Send(query);
                if (!result.Success)
                    return NotFound(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment with ID: {Id}", id);
                return StatusCode(500, Result<PaymentDTO>.Fail(
                    message: "حدث خطأ أثناء استرجاع الدفع",
                    errorType: "GetPaymentFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<PaymentDTO>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
                var query = new GetAllPaymentsQuery(parameters);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all payments");
                return StatusCode(500, Result<PaginatedResult<PaymentDTO>>.Fail(
                    message: "حدث خطأ أثناء استرجاع المدفوعات",
                    errorType: "GetPaymentsFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, [FromBody] CreatePaymentDTO dto)
        {
            try
            {
                var command = new UpdatePaymentCommand(id, dto.OrderId, dto.Amount, dto.PaymentMethodId, dto.StatusId);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment with ID: {Id}", id);
                return StatusCode(500, Result<bool>.Fail(
                    message: "حدث خطأ أثناء تحديث الدفع",
                    errorType: "UpdatePaymentFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            try
            {
                var command = new DeletePaymentCommand(id);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting payment with ID: {Id}", id);
                return StatusCode(500, Result<bool>.Fail(
                    message: "حدث خطأ أثناء حذف الدفع",
                    errorType: "DeletePaymentFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }
    }
}
