using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetProductsByUserId;

public record GetProductsByUserIdQuery(Guid UserId, PaginationParameters Parameters) : IRequest<Result<PaginatedResult<ProductDTO>>>; 