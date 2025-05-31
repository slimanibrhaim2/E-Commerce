using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetServicesByDuration;

public record GetServicesByDurationQuery(int MinDuration, int MaxDuration, int PageNumber = 1, int PageSize = 10) : IRequest<Result<PaginatedResult<ServiceDTO>>>; 