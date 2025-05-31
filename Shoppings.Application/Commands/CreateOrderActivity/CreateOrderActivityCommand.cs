using MediatR;
using Shoppings.Domain.Entities;

namespace Shoppings.Application.Commands
{
    public record CreateOrderActivityCommand(Guid Status) : IRequest<OrderActivity>;
} 