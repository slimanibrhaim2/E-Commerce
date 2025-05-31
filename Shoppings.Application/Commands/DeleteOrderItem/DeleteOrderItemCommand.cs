using MediatR;
using Core.Result;

namespace Shoppings.Application.Commands.DeleteOrderItem
{
    public record DeleteOrderItemCommand(Guid Id) : IRequest<Result<bool>>;
} 