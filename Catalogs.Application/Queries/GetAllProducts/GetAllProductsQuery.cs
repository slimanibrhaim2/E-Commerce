using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetAllProducts;

public record GetAllProductsQuery(PaginationParameters Pagination) : IRequest<Result<PaginatedResult<ProductDTO>>>; 