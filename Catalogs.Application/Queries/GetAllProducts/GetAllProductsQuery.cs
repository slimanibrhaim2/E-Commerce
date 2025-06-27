using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetAllProducts;

public record GetAllProductsQuery(PaginationParameters Pagination, Guid UserId) : IRequest<Result<PaginatedResult<ProductDTO>>>; 