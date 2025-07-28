using MediatR;
using Core.Result;
using Core.Pagination;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Queries.GetProductsByAdvancedFilters
{
    public record GetProductsByAdvancedFiltersQuery(
        Guid? CategoryId,
        decimal? MinPrice,
        decimal? MaxPrice,
        List<FeatureFilterDTO>? Features,
        Guid UserId,
        int PageNumber,
        int PageSize
    ) : IRequest<Result<PaginatedResult<ProductDTO>>>;
} 