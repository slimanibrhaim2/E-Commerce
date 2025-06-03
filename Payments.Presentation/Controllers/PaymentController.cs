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

namespace Payments.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> Create([FromBody] CreatePaymentDTO dto)
        {
            var command = new CreatePaymentCommand(dto.OrderId, dto.Amount, dto.PaymentMethodId, dto.StatusId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, [FromBody] CreatePaymentDTO dto)
        {
            var command = new UpdatePaymentCommand(id, dto.OrderId, dto.Amount, dto.PaymentMethodId, dto.StatusId);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeletePaymentCommand(id);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<Payment>>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            var query = new GetAllPaymentsQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<Payment>>> GetById(Guid id)
        {
            var query = new GetPaymentByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
