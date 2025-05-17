// Users.API/Controllers/FollowersController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Users.Application.Command.AddFollowerByUserId;
using Users.Application.Queries.GetAllFollowersByUserId;
using Core.Result;
using System.Collections.Generic;
using Users.Application.DTOs;

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
                return StatusCode(500, Result.Fail(
                    message: "فشل في إضافة المتابع",
                    errorType: "AddFollowerFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result.Ok(
                message: "تم إضافة المتابع بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid userId)
        {
            var query = new GetAllFollowersByUserIdQuery(userId);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب المتابعين",
                    errorType: "GetFollowersFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<IEnumerable<FollowerDTO>>.Ok(
                data: result.Data,
                message: "تم جلب المتابعين بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
}
