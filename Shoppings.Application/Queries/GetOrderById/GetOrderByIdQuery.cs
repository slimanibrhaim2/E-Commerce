using MediatR;
using Core.Result;
using Shoppings.Application.DTOs;

namespace Shoppings.Application.Queries.GetOrderById
{
    public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderWithItemsDTO>>;
} 