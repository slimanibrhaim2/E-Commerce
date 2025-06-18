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
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;
using Core.Result;

namespace Catalogs.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CouponController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CouponController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCouponDTO dto)
        {
            var userId = User.GetId();
            dto.UserId = userId;
            var command = new CreateCouponCommand(
                dto.UserId,
                dto.Code,
                dto.DiscountAmount,
                dto.ExpiryDate
            );
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في إنشاء الكوبون",
                    errorType: "CreateCouponFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result.Ok(
                message: "تم إنشاء الكوبون بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<Result<List<CouponDTO>>>> GetAll()
        {
            var query = new GetAllCouponsQuery();
            var result = await _mediator.Send(query);
            return Ok(Result<List<CouponDTO>>.Ok(
                data: result.Data,
                message: "تم جلب الكوبونات بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Result<CouponDTO>>> GetById(Guid id)
        {
            var query = new GetCouponByIdQuery(id);
            var result = await _mediator.Send(query);
            if (result == null)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب الكوبون",
                    errorType: "GetCouponByIdFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<CouponDTO>.Ok(
                data: result.Data,
                message: "تم جلب الكوبون بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateCouponDTO dto)
        {
            var userId = User.GetId();
            var command = new UpdateCouponCommand(
                id,
                userId,
                dto.Code,
                dto.DiscountAmount,
                dto.ExpiryDate
            );
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث الكوبون",
                    errorType: "UpdateCouponFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result.Ok(
                message: "تم تحديث الكوبون بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteCouponCommand(id);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف الكوبون",
                    errorType: "DeleteCouponFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result.Ok(
                message: "تم حذف الكوبون بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
} 