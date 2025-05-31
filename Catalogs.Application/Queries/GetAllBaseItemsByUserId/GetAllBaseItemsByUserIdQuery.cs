using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using System.Collections.Generic;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetAllBaseItemsByUserId;

public record GetAllBaseItemsByUserIdQuery(Guid UserId, PaginationParameters Parameters) : IRequest<Result<PaginatedResult<BaseItemDTO>>>; 