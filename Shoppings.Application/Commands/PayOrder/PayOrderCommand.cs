using MediatR;
using System;
using Core.Result;

namespace Shoppings.Application.Commands.PayOrder
{
    public record PayOrderCommand(Guid OrderId) : IRequest<Result>;
} 