// Users.Application/Commands/AddAddressByUserId/AddAddressByUserIdCommand.cs
using MediatR;
using System;
using Users.Application.DTOs;
using Core.Result;

namespace Users.Application.Commands.AddAddressByUserId
{
    public record AddAddressByUserIdCommand(
        Guid UserId,
        AddressDTO Address
    ) : IRequest<Result>;
}
