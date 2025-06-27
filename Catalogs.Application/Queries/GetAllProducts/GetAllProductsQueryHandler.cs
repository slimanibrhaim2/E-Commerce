using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Core.Pagination;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<PaginatedResult<ProductDTO>>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetAllProductsQueryHandler> _logger;
    private readonly IFeatureRepository _featureRepo;
    private readonly IFavoriteRepository _favoriteRepo;

    public GetAllProductsQueryHandler(
        IProductRepository repo,
        ILogger<GetAllProductsQueryHandler> logger,
        IFeatureRepository featureRepo,
        IFavoriteRepository favoriteRepo)
    {
        _repo = repo;
        _logger = logger;
        _featureRepo = featureRepo;
        _favoriteRepo = favoriteRepo;
    }

    public async Task<Result<PaginatedResult<ProductDTO>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _repo.GetAllAsync();
            var productsList = products.ToList();

            // Get all favorites for the user if authenticated
            var userFavorites = request.UserId != Guid.Empty 
                ? await _favoriteRepo.GetFavoritesByUserIdAsync(request.UserId)
                : new List<Domain.Entities.Favorite>();

            // Get all favorite base item IDs for quick lookup
            var favoriteBaseItemIds = userFavorites.Select(f => f.BaseItemId).ToHashSet();

            // Apply pagination
            var totalCount = productsList.Count;
            var pagedProducts = productsList
                .Skip((request.Pagination.PageNumber - 1) * request.Pagination.PageSize)
                .Take(request.Pagination.PageSize)
                .ToList();

            // Map to DTOs and include features
            var productDTOs = new List<ProductDTO>();
            foreach (var product in pagedProducts)
            {
                var features = await _featureRepo.GetProductFeaturesByEntityIdAsync(product.Id);
                var productDTO = new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                    SKU = product.SKU,
                    StockQuantity = product.StockQuantity,
                    IsAvailable = product.IsAvailable,
                    UserId = product.UserId,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    Media = product.Media?.Select(m => new MediaDTO
                    {
                        Id = m.Id,
                        Url = m.MediaUrl,
                        MediaTypeId = m.MediaTypeId,
                        BaseItemId = m.BaseItemId
                    }).ToList() ?? new List<MediaDTO>(),
                    Features = features.Select(f => new ProductFeatureDTO
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Value = f.Value
                    }).ToList(),
                    IsFavorite = favoriteBaseItemIds.Contains(product.BaseItemId)
                };
                productDTOs.Add(productDTO);
            }

            var paginatedResult = PaginatedResult<ProductDTO>.Create(
                data: productDTOs,
                pageNumber: request.Pagination.PageNumber,
                pageSize: request.Pagination.PageSize,
                totalCount: totalCount
            );

            _logger.LogInformation("تم جلب {Count} منتج", productDTOs.Count);

            return Result<PaginatedResult<ProductDTO>>.Ok(
                data: paginatedResult,
                message: "تم جلب المنتجات بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل في جلب المنتجات");
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: $"فشل في جلب المنتجات: {ex.Message}",
                errorType: "GetAllProductsFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
}