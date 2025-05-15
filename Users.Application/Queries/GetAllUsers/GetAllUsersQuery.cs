// Users.Application/Queries/GetAllUsers/GetAllUsersQuery.cs
using MediatR;
using System.Collections.Generic;
using Users.Application.DTOs;
using Core.Result;

namespace Users.Application.Queries.GetAllUsers
{
    // Now returns a Core.Result<IEnumerable<UserDTO>>
    public record GetAllUsersQuery() : IRequest<Result<IEnumerable<UserDTO>>>;
}
