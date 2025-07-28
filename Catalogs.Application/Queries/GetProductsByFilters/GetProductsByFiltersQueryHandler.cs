using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Core.Pagination;
using System.Data;

namespace Catalogs.Application.Queries.GetProductsByFilters;

public class GetProductsByFiltersQueryHandler : IRequestHandler<GetProductsByFiltersQuery, Result<PaginatedResult<ProductDTO>>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetProductsByFiltersQueryHandler> _logger;
    private readonly IFeatureRepository _featureRepo;
    private readonly IFavoriteRepository _favoriteRepo;

    public GetProductsByFiltersQueryHandler(
        IProductRepository repo,
        ILogger<GetProductsByFiltersQueryHandler> logger,
        IFeatureRepository featureRepo,
        IFavoriteRepository favoriteRepo)
    {
        _repo = repo;
        _logger = logger;
        _featureRepo = featureRepo;
        _favoriteRepo = favoriteRepo;
    }

    public async Task<Result<PaginatedResult<ProductDTO>>> Handle(GetProductsByFiltersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get products with filters
            var paginatedProducts = await _repo.GetByFiltersWithDetails(
                request.CategoryId,
                request.MinPrice,
                request.MaxPrice,
                request.PageNumber,
                request.PageSize);

            // Get all favorites for the user if authenticated
            var userFavorites = request.UserId != Guid.Empty 
                ? await _favoriteRepo.GetFavoritesByUserIdAsync(request.UserId)
                : new List<Domain.Entities.Favorite>();

            // Get all favorite base item IDs for quick lookup
            var favoriteBaseItemIds = userFavorites.Select(f => f.BaseItemId).ToHashSet();

            // Map to DTOs and include features
            var productDTOs = new List<ProductDTO>();
            foreach (var product in paginatedProducts.Data)
            {
                var features = await _featureRepo.GetProductFeaturesByEntityIdAsync(product.Id);
                var productDTO = new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.Name,
                    SKU = product.SKU,
                    SerialNumber = product.SerialNumber,
                    StockQuantity = product.StockQuantity,
                    IsAvailable = product.IsAvailable,
                    UserId = product.UserId,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    Media = product.Media?.Select(m => new MediaDTO
                    {
                        Url = m.MediaUrl,
                        MediaTypeName = m.MediaType?.Name ?? "unknown"
                    }).ToList() ?? new List<MediaDTO>(),
                    Features = features.Select(f => new ProductFeatureDTO
                    {
                        Name = f.Name,
                        Value = f.Value
                    }).ToList(),
                    IsFavorite = favoriteBaseItemIds.Contains(product.BaseItemId)
                };
                productDTOs.Add(productDTO);
            }

            var paginatedResult = PaginatedResult<ProductDTO>.Create(
                data: productDTOs,
                pageNumber: paginatedProducts.PageNumber,
                pageSize: paginatedProducts.PageSize,
                totalCount: paginatedProducts.TotalCount
            );

            _logger.LogInformation("تم جلب {Count} منتج بالفلاتر المحددة", productDTOs.Count);

            return Result<PaginatedResult<ProductDTO>>.Ok(
                data: paginatedResult,
                message: "تم جلب المنتجات بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل في جلب المنتجات بالفلاتر المحددة");
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: $"فشل في جلب المنتجات: {ex.Message}",
                errorType: "GetFilteredProductsFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 