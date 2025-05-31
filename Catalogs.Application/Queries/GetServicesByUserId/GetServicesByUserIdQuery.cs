using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetServicesByUserId;

public record GetServicesByUserIdQuery(Guid UserId, PaginationParameters Parameters) : IRequest<Result<PaginatedResult<ServiceDTO>>>; 