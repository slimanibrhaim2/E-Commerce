using MediatR;
using Shoppings.Domain.Entities;
using Core.Result;

namespace Shoppings.Application.Queries.GetOrderActivityById
{
    public record GetOrderActivityByIdQuery(Guid Id) : IRequest<Result<OrderActivity>>;
} 