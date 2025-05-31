using MediatR;
using Shoppings.Domain.Entities;
using Core.Result;

namespace Shoppings.Application.Commands.CreateOrderItem
{
    public record CreateOrderItemCommand(Guid OrderId, Guid BaseItemId, double Quantity, double Price, Guid CouponId) : IRequest<Result<Guid>>;
} 