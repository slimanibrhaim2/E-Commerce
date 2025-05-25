using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetProductsByBrand;

public class GetProductsByBrandQueryHandler : IRequestHandler<GetProductsByBrandQuery, Result<PaginatedResult<ProductDTO>>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetProductsByBrandQueryHandler> _logger;

    public GetProductsByBrandQueryHandler(IProductRepository repo, ILogger<GetProductsByBrandQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ProductDTO>>> Handle(GetProductsByBrandQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _repo.GetByBrand(request.BrandId);
            var productDtos = products.Select(p => new ProductDTO
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
            var totalCount = productDtos.Count;

            var paginatedProducts = PaginatedResult<ProductDTO>.Create(
                data: productDtos,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                totalCount: totalCount);

            return Result<PaginatedResult<ProductDTO>>.Ok(
                data: paginatedProducts,
                message: "تم جلب المنتجات بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products by brand {BrandId}", request.BrandId);
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: "حدث خطأ أثناء جلب المنتجات",
                errorType: "GetProductsByBrandFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 