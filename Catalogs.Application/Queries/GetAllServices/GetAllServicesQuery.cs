using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetAllServices;

public record GetAllServicesQuery(PaginationParameters Pagination) : IRequest<Result<PaginatedResult<ServiceDTO>>>; 