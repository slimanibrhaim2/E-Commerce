using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetProductsByPriceRange;

public class GetProductsByPriceRangeQueryHandler : IRequestHandler<GetProductsByPriceRangeQuery, Result<PaginatedResult<ProductDTO>>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetProductsByPriceRangeQueryHandler> _logger;

    public GetProductsByPriceRangeQueryHandler(IProductRepository repo, ILogger<GetProductsByPriceRangeQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ProductDTO>>> Handle(GetProductsByPriceRangeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.MinPrice > request.MaxPrice)
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "السعر الأدنى يجب أن يكون أقل من السعر الأقصى",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);

            var products = await _repo.GetByPriceRange(request.MinPrice, request.MaxPrice, request.PageNumber, request.PageSize);
            var productDtos = products.Data.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                SKU = p.SKU,
                StockQuantity = p.StockQuantity,
                IsAvailable = p.IsAvailable,
                // Add other properties as needed
            }).ToList();

            var paginatedProducts = PaginatedResult<ProductDTO>.Create(
                data: productDtos,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                totalCount: products.TotalCount);

            return Result<PaginatedResult<ProductDTO>>.Ok(
                data: paginatedProducts,
                message: "تم جلب المنتجات بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products in price range {MinPrice} - {MaxPrice}", request.MinPrice, request.MaxPrice);
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: "حدث خطأ أثناء جلب المنتجات",
                errorType: "GetProductsByPriceRangeFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 