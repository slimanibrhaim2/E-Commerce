using MediatR;
using Core.Result;
using System;

namespace Shoppings.Application.Commands.Checkout
{
    public record CheckoutCommand(Guid CartId, Guid AddressId) : IRequest<Result<Guid>>;
} 