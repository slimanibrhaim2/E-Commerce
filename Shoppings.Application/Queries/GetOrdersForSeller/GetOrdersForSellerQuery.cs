using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Application.DTOs;

namespace Shoppings.Application.Queries.GetOrdersForSeller;

public record GetOrdersForSellerQuery(
    Guid SellerId,
    PaginationParameters Parameters
) : IRequest<Result<PaginatedResult<SellerOrderDTO>>>; 