using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Application.DTOs;

namespace Shoppings.Application.Queries.GetMyOrders
{
    public record GetMyOrdersQuery(Guid UserId, PaginationParameters Parameters) : IRequest<Result<PaginatedResult<MyOrderDTO>>>;
} 