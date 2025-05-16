// Users.Application/Commands/CreateUser/CreateUserCommand.cs
using MediatR;
using System;
using Users.Application.DTOs;
using Core.Result;

namespace Users.Application.Commands.CreateUser
{
    public record CreateUserCommand(
        string FirstName,
        string? MiddleName,
        string LastName,
        string PhoneNumber,
        string Email,
        string? ProfilePhoto,
        string? Description
    ) : IRequest<Result<Guid>>;
}
