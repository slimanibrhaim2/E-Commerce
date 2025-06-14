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
using Users.Application.Commands.DeleteFollower;
using Core.Pagination;
using Microsoft.AspNetCore.Authorization;
using Core.Authentication;

namespace Users.Presentation.Controllers
{
    [ApiController]
    [Route("api/users/followers")]
    [Authorize]
    public class FollowersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FollowersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Guid followerId)
        {
            var userId = User.GetId();
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
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.GetId();
            var pagination = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllFollowersByUserIdQuery(userId, pagination);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب المتابعين",
                    errorType: "GetFollowersFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<PaginatedResult<FollowerDTO>>.Ok(
                data: result.Data,
                message: "تم جلب المتابعين بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("{followerId}/{followingId}")]
        public async Task<IActionResult> DeleteFollower(Guid followingId)
        {
            // Get the current user's ID
            var currentUserId = User.GetId();

            var cmd = new DeleteFollowerCommand(currentUserId, followingId);
            var result = await _mediator.Send(cmd);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف المتابع",
                    errorType: "DeleteFollowerFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result.Ok(
                message: "تم حذف المتابع بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
}
