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

namespace Users.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AddressesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> AddAddress(Guid userId, [FromBody] AddAddressDTO addressDTO)
        {
            var command = new AddAddressByUserIdCommand(userId, addressDTO);
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{userId}/addresses")]
        public async Task<IActionResult> GetAddresses(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pagination = new Core.Models.PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAddressesByUserIdQuery(userId, pagination);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب العناوين",
                    errorType: "GetAddressesFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<Core.Models.PaginatedResult<AddressDTO>>.Ok(
                data: result.Data,
                message: "تم جلب العناوين بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("{addressId}")]
        public async Task<IActionResult> DeleteAddress(Guid addressId)
        {
            var command = new DeleteAddressCommand(addressId);
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
