using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Domain.Entities;

namespace Shoppings.Application.Queries.GetAllCart
{
    public record GetAllCartQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<Cart>>>;
} 