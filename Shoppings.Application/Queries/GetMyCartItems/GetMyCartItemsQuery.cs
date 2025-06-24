using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Application.DTOs;

namespace Shoppings.Application.Queries.GetMyCartItems
{
    public record GetMyCartItemsQuery(Guid UserId, PaginationParameters Parameters) : IRequest<Result<PaginatedResult<CartItemDTO>>>;
} 