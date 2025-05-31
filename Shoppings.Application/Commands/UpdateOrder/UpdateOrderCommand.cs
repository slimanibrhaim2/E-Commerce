using MediatR;
using Shoppings.Domain.Entities;
using Core.Result;

namespace Shoppings.Application.Commands.UpdateOrder
{
    public record UpdateOrderCommand(Guid Id, Guid OrderActivityId) : IRequest<Result<bool>>;
} 