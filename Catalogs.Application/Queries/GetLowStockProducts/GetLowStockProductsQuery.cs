using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetLowStockProducts;

public record GetLowStockProductsQuery(int Threshold = 10, int PageNumber = 1, int PageSize = 10) : IRequest<Result<PaginatedResult<ProductDTO>>>; 