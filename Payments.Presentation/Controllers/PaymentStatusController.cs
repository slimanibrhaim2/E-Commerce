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

namespace Payments.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentStatusController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PaymentStatusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> Create([FromBody] CreatePaymentStatusDTO dto)
        {
            var command = new CreatePaymentStatusCommand(dto.Name);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Result<bool>>> Update(Guid id, [FromBody] CreatePaymentStatusDTO dto)
        {
            var command = new UpdatePaymentStatusCommand(id, dto.Name);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Result<bool>>> Delete(Guid id)
        {
            var command = new DeletePaymentStatusCommand(id);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<Result<PaginatedResult<PaymentStatus>>>> GetAll([FromQuery] PaginationParameters parameters)
        {
            var query = new GetAllPaymentStatusesQuery(parameters);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<PaymentStatus>>> GetById(Guid id)
        {
            var query = new GetPaymentStatusByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
