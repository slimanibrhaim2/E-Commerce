// Users.Application/Commands/DeleteUser/DeleteUserCommand.cs
using Core.Result;
using MediatR;
using System;

namespace Users.Application.Commands.DeleteUser
{
    public record DeleteUserCommand(Guid Id) : IRequest<Result>;
}
