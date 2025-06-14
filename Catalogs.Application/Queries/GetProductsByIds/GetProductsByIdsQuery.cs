using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetProductsByIds;

public record GetProductsByIdsQuery(
    IEnumerable<Guid> Ids,
    PaginationParameters Parameters) : IRequest<Result<PaginatedResult<ProductDTO>>>; 