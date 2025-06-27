using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetProductsByName;

public record GetProductsByNameQuery(string Name, Guid UserId, PaginationParameters Parameters) : IRequest<Result<PaginatedResult<ProductDTO>>>; 