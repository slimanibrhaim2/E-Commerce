using MediatR;
using Microsoft.AspNetCore.Mvc;
using Catalogs.Application.Commands.CreateCoupon;
using Catalogs.Application.Commands.UpdateCoupon;
using Catalogs.Application.Commands.DeleteCoupon;
using Catalogs.Application.Queries.GetAllCoupons;
using Catalogs.Application.Queries.GetCouponById;
using Catalogs.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CouponController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCouponCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCouponCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCouponCommand(id));
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllCouponsQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetCouponByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
} 