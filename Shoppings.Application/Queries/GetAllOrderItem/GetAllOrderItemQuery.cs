using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Application.DTOs;

namespace Shoppings.Application.Queries.GetAllOrderItem
{
    public record GetAllOrderItemQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<OrderItemWithDetailsDTO>>>;
} 