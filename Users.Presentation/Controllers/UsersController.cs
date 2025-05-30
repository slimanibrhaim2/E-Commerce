﻿// Users.API/Controllers/UsersController.cs
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
namespace Users.Presentation.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO dto)
        {
            var result = await _mediator.Send(new CreateUserCommand(dto));
            if (!result.Success) 
                return StatusCode(500, Result.Fail(
                    message: "فشل في إنشاء المستخدم",
                    errorType: "CreateUserFailed",
                    resultStatus: ResultStatus.Failed));
            return CreatedAtAction(nameof(GetById), new { id = result.Data }, Result.Ok(
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetUserByIdQuery(id);
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

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateUserDTO dto)
        {
            
            var result = await _mediator.Send(new UpdateUserCommand(id,dto));
            if (!result.Success)
                return StatusCode(500, Result.Fail(
                    message: "فشل في تحديث المستخدم",
                    errorType: "UpdateUserFailed",
                    resultStatus: ResultStatus.Failed));
            return Ok(Result.Ok(
                message: "تم تحديث المستخدم بنجاح",
                resultStatus: ResultStatus.Success));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id));
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
