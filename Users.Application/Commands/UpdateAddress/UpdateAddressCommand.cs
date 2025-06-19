using MediatR;
using System;
using Users.Application.DTOs;
using Core.Result;

namespace Users.Application.Commands.UpdateAddress
{
    public record UpdateAddressCommand(
        Guid AddressId,
        AddAddressDTO AddressDTO
    ) : IRequest<Result>;
} 