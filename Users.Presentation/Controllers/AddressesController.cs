// Users.API/Controllers/AddressesController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Users.Application.DTOs;
using Users.Application.Commands.AddAddressByUserId;
using Users.Application.Queries.GetAddressesByUserId;
using Core.Result;
using Users.Application.Commands.DeleteAddress;
using Core.Pagination;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;

namespace Users.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AddressesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AddressesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] AddAddressDTO addressDTO)
        {
            var userId = User.GetId();
            var command = new AddAddressByUserIdCommand(userId, addressDTO);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في إضافة العنوان",
                    errorType: "AddAddressFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result.Ok(
                message: "تم إضافة العنوان بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet]
        public async Task<IActionResult> GetAddresses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.GetId();
            var pagination = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAddressesByUserIdQuery(userId, pagination);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب العناوين",
                    errorType: "GetAddressesFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<PaginatedResult<AddressDTO>>.Ok(
                data: result.Data,
                message: "تم جلب العناوين بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("{addressId}")]
        public async Task<IActionResult> DeleteAddress(Guid addressId)
        {
            var command = new DeleteAddressCommand(addressId);
            var result = await _mediator.Send(command);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف العنوان",
                    errorType: "DeleteAddressFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result.Ok(
                message: "تم حذف العنوان بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
}
