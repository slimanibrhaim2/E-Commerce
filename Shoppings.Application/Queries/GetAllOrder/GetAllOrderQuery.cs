using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Domain.Entities;

namespace Shoppings.Application.Queries.GetAllOrder
{
    public record GetAllOrderQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<Order>>>;
} 