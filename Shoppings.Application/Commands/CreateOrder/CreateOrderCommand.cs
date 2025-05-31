using MediatR;
using Shoppings.Domain.Entities;
using Core.Result;

namespace Shoppings.Application.Commands.CreateOrder
{
    public record CreateOrderCommand(Guid UserId, Guid OrderActivityId) : IRequest<Result<Guid>>;
} 