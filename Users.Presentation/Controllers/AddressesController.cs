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
        public async Task<IActionResult> Add(Guid userId, [FromBody] AddressDTO dto)
        {
            var cmd = new AddAddressByUserIdCommand(userId, dto);
            var result = await _mediator.Send(cmd);
            if (!result.Success)
                return result.ResultStatus == ResultStatus.ValidationError
                    ? NotFound(result)
                    : StatusCode(500, result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid userId)
        {
            var query = new GetAddressesByUserIdQuery(userId);
            var result = await _mediator.Send(query);
            return result.Success
                ? Ok(result.Data)
                : result.ResultStatus == ResultStatus.ValidationError
                    ? NotFound(result)
                    : StatusCode(500, result);
        }
    }
}
