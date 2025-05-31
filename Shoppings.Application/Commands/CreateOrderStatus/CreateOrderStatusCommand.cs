using MediatR;
using Shoppings.Domain.Entities;

namespace Shoppings.Application.Commands
{
    public record CreateOrderStatusCommand(string Name) : IRequest<OrderStatus>;
} 