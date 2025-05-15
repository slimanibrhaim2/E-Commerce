// Users.Application/Commands/CreateUser/CreateUserCommand.cs
using MediatR;
using System;
using Users.Application.DTOs;
using Core.Result;

namespace Users.Application.Commands.CreateUser
{
    public record CreateUserCommand(
        UserDTO User,
        Guid CorrelationId = default
    ) : IRequest<Result<Guid>>;
}
