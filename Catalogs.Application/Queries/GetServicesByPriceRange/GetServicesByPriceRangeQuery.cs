using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetServicesByPriceRange;

public record GetServicesByPriceRangeQuery(decimal MinPrice, decimal MaxPrice, Guid UserId, int PageNumber = 1, int PageSize = 10) : IRequest<Result<PaginatedResult<ServiceDTO>>>; 