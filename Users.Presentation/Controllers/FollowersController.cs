// Users.API/Controllers/FollowersController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Users.Application.Command.AddFollowerByUserId;
using Users.Application.Queries.GetAllFollowersByUserId;
using Core.Result;

namespace Users.Presentation.Controllers
{
    [ApiController]
    [Route("api/users/{userId:guid}/followers")]
    public class FollowersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FollowersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Add(Guid userId, [FromBody] Guid followerId)
        {
            var cmd = new AddFollowerByUserIdCommand(userId, followerId);
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
            var query = new GetAllFollowersByUserIdQuery(userId);
            var result = await _mediator.Send(query);
            return result.Success
                ? Ok(result.Data)
                : result.ResultStatus == ResultStatus.ValidationError
                    ? NotFound(result)
                    : StatusCode(500, result);
        }
    }
}
