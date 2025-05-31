using MediatR;
using Shoppings.Domain.Entities;
using System.Collections.Generic;
using Core.Result;
using Core.Pagination;

namespace Shoppings.Application.Queries.GetAllOrderStatus
{
    public record GetAllOrderStatusQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<OrderStatus>>>;
} 