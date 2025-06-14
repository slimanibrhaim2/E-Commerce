using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetServicesByIds;

public record GetServicesByIdsQuery(
    IEnumerable<Guid> Ids,
    PaginationParameters Parameters) : IRequest<Result<PaginatedResult<ServiceDTO>>>; 