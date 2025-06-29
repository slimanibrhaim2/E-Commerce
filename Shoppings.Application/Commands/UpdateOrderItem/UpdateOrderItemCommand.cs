using MediatR;
using Core.Result;

namespace Shoppings.Application.Commands.UpdateOrderItem
{
    public record UpdateOrderItemCommand(Guid Id, double Quantity, double Price) : IRequest<Result<bool>>;
} 