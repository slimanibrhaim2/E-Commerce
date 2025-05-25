using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetProductsByBrand;

public record GetProductsByBrandQuery(Guid BrandId, int PageNumber = 1, int PageSize = 10) : IRequest<Result<PaginatedResult<ProductDTO>>>; 