using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Application.DTOs;

namespace Shoppings.Application.Queries.GetAllOrder
{
    public record GetAllOrderQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<MyOrderDTO>>>;
} 