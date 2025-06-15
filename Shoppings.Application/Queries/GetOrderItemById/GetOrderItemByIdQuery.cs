using MediatR;
using Shoppings.Domain.Entities;
using Core.Result;

namespace Shoppings.Application.Queries.GetOrderItemById
{
    public record GetOrderItemByIdQuery(Guid Id) : IRequest<Result<OrderItem>>;
} 