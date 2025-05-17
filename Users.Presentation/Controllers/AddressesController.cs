// Users.API/Controllers/AddressesController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Users.Application.DTOs;
using Users.Application.Commands.AddAddressByUserId;
using Users.Application.Queries.GetAddressesByUserId;
using Core.Result;

namespace Users.Presentation.Controllers
{
    [ApiController]
    [Route("api/users/{userId:guid}/addresses")]
    public class AddressesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AddressesController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Add(Guid userId, [FromBody] AddAddressDTO dto)
        {
            var cmd = new AddAddressByUserIdCommand(userId, dto);
            var result = await _mediator.Send(cmd);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في إضافة العنوان",
                    errorType: "AddAddressFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result.Ok(
                message: "تم إضافة العنوان بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("{userId}/addresses")]
        public async Task<IActionResult> GetAddresses(Guid userId)
        {
            var query = new GetAddressesByUserIdQuery(userId);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب العناوين",
                    errorType: "GetAddressesFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<IEnumerable<AddressDTO>>.Ok(
                data: result.Data,
                message: "تم جلب العناوين بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
}
