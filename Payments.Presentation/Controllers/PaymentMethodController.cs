using MediatR;
using Microsoft.AspNetCore.Mvc;
using Payments.Application.Commands.CreatePaymentMethod;
using Payments.Application.Commands.UpdatePaymentMethod;
using Payments.Application.Commands.DeletePaymentMethod;
using Payments.Application.DTOs;
using Payments.Application.Queries.GetAllPaymentMethods;
using Payments.Application.Queries.GetPaymentMethodById;
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
    public class PaymentMethodController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentMethodController> _logger;

        public PaymentMethodController(IMediator mediator, ILogger<PaymentMethodController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> Create([FromBody] CreatePaymentMethodDTO dto)
        {
            try
            {
                var command = new CreatePaymentMethodCommand(dto.Name, dto.Description, dto.IsActive);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return BadRequest(result);
                return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment method");
                return StatusCode(500, Result<Guid>.Fail(
                    message: "حدث خطأ أثناء إنشاء طريقة الدفع",
                    errorType: "CreatePaymentMethodFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<PaymentMethodDTO>>> GetById(Guid id)
        {
            try
            {
                var query = new GetPaymentMethodByIdQuery(id);
                var result = await _mediator.Send(query);
                if (!result.Success)
                    return NotFound(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment method with ID: {Id}", id);
                return StatusCode(500, Result<PaymentMethodDTO>.Fail(
                    message: "حدث خطأ أثناء استرجاع طريقة الدفع",
                    errorType: "GetPaymentMethodFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<PaymentMethodDTO>>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
                var query = new GetAllPaymentMethodsQuery(parameters);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all payment methods");
                return StatusCode(500, Result<PaginatedResult<PaymentMethodDTO>>.Fail(
                    message: "حدث خطأ أثناء استرجاع طرق الدفع",
                    errorType: "GetPaymentMethodsFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, [FromBody] CreatePaymentMethodDTO dto)
        {
            try
            {
                var command = new UpdatePaymentMethodCommand(id, dto.Name, dto.Description, dto.IsActive);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment method with ID: {Id}", id);
                return StatusCode(500, Result<bool>.Fail(
                    message: "حدث خطأ أثناء تحديث طريقة الدفع",
                    errorType: "UpdatePaymentMethodFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            try
            {
                var command = new DeletePaymentMethodCommand(id);
                var result = await _mediator.Send(command);
                if (!result.Success)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting payment method with ID: {Id}", id);
                return StatusCode(500, Result<bool>.Fail(
                    message: "حدث خطأ أثناء حذف طريقة الدفع",
                    errorType: "DeletePaymentMethodFailed",
                    resultStatus: ResultStatus.Failed));
            }
        }
    }
}
