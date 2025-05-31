using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Domain.Entities;

namespace Shoppings.Application.Queries.GetAllCartItem
{
    public record GetAllCartItemQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<CartItem>>>;
} 