using MediatR;
using Core.Result;
using System;

namespace Shoppings.Application.Commands.TransactCartToOrder
{
    public record TransactCartToOrderCommand(Guid CartId) : IRequest<Result<Guid>>;
} 