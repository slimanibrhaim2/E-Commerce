using MediatR;
using Shoppings.Application.DTOs;
using Core.Result;

namespace Shoppings.Application.Queries.GetOrderItemById
{
    public record GetOrderItemByIdQuery(Guid Id) : IRequest<Result<OrderItemWithDetailsDTO>>;
} 