using Core.Result;
using MediatR;
using Shoppings.Domain.Entities;

namespace Shoppings.Application.Queries.GetOrderStatusById
{
    public record GetOrderStatusByIdQuery(Guid Id) : IRequest<Result<OrderStatus>>;
} 