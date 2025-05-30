using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetServicesByName;

public record GetServicesByNameQuery(string Name, PaginationParameters Parameters) : IRequest<Result<PaginatedResult<ServiceDTO>>>; 