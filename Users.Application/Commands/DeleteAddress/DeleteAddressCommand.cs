using MediatR;
using System;
using Core.Result;

namespace Users.Application.Commands.DeleteAddress
{
    public record DeleteAddressCommand(Guid AddressId) : IRequest<Result>;
} 