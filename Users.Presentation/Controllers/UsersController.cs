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
using Core.Pagination;
using Users.Application.Queries.GetUsersByName;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Core.Authentication;

namespace Users.Presentation.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO dto)
        {
            var result = await _mediator.Send(new CreateUserCommand(dto));
            if (!result.Success) 
                return StatusCode(500, Result.Fail(
                    message: "فشل في إنشاء المستخدم",
                    errorType: "CreateUserFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<Guid>.Ok(
                data: result.Data,
                message: "تم إنشاء المستخدم بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pagination = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetAllUsersQuery(pagination);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب المستخدمين",
                    errorType: "GetAllFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<PaginatedResult<UserDTO>>.Ok(
                data: result.Data,
                message: "تم جلب المستخدمين بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.GetId();
            
            var query = new GetUserByIdQuery(userId);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في جلب بيانات المستخدم",
                    errorType: "GetByIdFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<UserDTO>.Ok(
                data: result.Data,
                message: "تم جلب بيانات المستخدم بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] CreateUserDTO dto)
        {
            var userId = User.GetId();

            var result = await _mediator.Send(new UpdateUserCommand(userId, dto));
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث المستخدم",
                    errorType: "UpdateUserFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result.Ok(
                message: "تم تحديث المستخدم بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteCurrentUser()
        {
            var userId = User.GetId();

            var result = await _mediator.Send(new DeleteUserCommand(userId));
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في حذف المستخدم",
                    errorType: "DeleteUserFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result.Ok(
                message: "تم حذف المستخدم بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string name, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var parameters = new PaginationParameters { PageNumber = pageNumber, PageSize = pageSize };
            var query = new GetUsersByNameQuery(name, parameters);
            var result = await _mediator.Send(query);
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في البحث عن المستخدمين",
                    errorType: "SearchByNameFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result<PaginatedResult<UserDTO>>.Ok(
                data: result.Data,
                message: "تم جلب المستخدمين بنجاح",
                resultStatus: ResultStatus.Success));
        }
    }
}
