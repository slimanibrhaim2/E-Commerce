using MediatR;
using Core.Result;

namespace Shoppings.Application.Commands.UpdateOrderItem
{
    public record UpdateOrderItemCommand(Guid Id, double Quantity, double Price, Guid CouponId) : IRequest<Result<bool>>;
} 