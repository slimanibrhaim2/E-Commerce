// Users.Application/Queries/GetUserById/GetUserByIdQuery.cs
using MediatR;
using System;
using Users.Application.DTOs;
using Core.Result;

namespace Users.Application.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId, Guid? RequestorUserId = null) : IRequest<Result<UserDTO>>;
