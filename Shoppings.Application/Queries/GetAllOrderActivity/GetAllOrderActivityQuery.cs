using MediatR;
using Shoppings.Domain.Entities;
using System.Collections.Generic;
using Core.Result;
using Core.Pagination;

namespace Shoppings.Application.Queries.GetAllOrderActivity
{
    public record GetAllOrderActivityQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<OrderActivity>>>;
} 