// Users.API/Controllers/UsersController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Users.Application.DTOs;
using Users.Application.Commands.CreateUser;
using Users.Application.Commands.DeleteUser;
using Users.Application.Queries.GetAllUsers;
using Users.Application.Queries.GetUserById;
using Core.Result;
using Users.Application.Command.UpdateUser;
namespace Users.Presentation.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDTO dto)
        {
            var result = await _mediator.Send(new CreateUserCommand(dto));
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Data }, null);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());
            return result.Success
                ? Ok(result.Data)
                : StatusCode(500, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));
            if (!result.Success)
                return result.ResultStatus == ResultStatus.ValidationError
                    ? NotFound(result)
                    : StatusCode(500, result);
            return Ok(result.Data);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserDTO dto)
        {
            if (id != dto.Id)
                return BadRequest(Result.Fail("Mismatched Id", "BadRequest", ResultStatus.ValidationError));

            var result = await _mediator.Send(new UpdateUserCommand(dto));
            if (!result.Success)
                return result.ResultStatus == ResultStatus.ValidationError
                    ? NotFound(result)
                    : StatusCode(500, result);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id));
            if (!result.Success)
                return result.ResultStatus == ResultStatus.ValidationError
                    ? NotFound(result)
                    : StatusCode(500, result);
            return NoContent();
        }
    }
}
