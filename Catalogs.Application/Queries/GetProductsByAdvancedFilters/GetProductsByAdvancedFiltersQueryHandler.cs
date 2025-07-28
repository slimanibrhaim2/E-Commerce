using MediatR;
using Core.Result;
using Core.Pagination;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Queries.GetProductsByAdvancedFilters
{
    public class GetProductsByAdvancedFiltersQueryHandler 
        : IRequestHandler<GetProductsByAdvancedFiltersQuery, Result<PaginatedResult<ProductDTO>>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IFeatureRepository _featureRepo;
        private readonly IFavoriteRepository _favoriteRepo;
        private readonly ILogger<GetProductsByAdvancedFiltersQueryHandler> _logger;

        public GetProductsByAdvancedFiltersQueryHandler(
            IProductRepository productRepository,
            IFeatureRepository featureRepo,
            IFavoriteRepository favoriteRepo,
            ILogger<GetProductsByAdvancedFiltersQueryHandler> logger)
        {
            _productRepository = productRepository;
            _featureRepo = featureRepo;
            _favoriteRepo = favoriteRepo;
            _logger = logger;
        }

        public async Task<Result<PaginatedResult<ProductDTO>>> Handle(
            GetProductsByAdvancedFiltersQuery request, 
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting products with advanced filters - CategoryId: {CategoryId}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}, Features: {FeaturesCount}",
                    request.CategoryId, request.MinPrice, request.MaxPrice, request.Features?.Count ?? 0);

                // Convert feature filters to tuple format for repository
                List<(string FeatureName, string? FeatureValue)>? featureFilters = null;
                if (request.Features != null && request.Features.Any())
                {
                    featureFilters = request.Features
                        .Select(f => (f.FeatureName, f.FeatureValue))
                        .ToList();
                }

                // Get products from repository
                var paginatedProducts = await _productRepository.GetByAdvancedFiltersWithDetails(
                    request.CategoryId,
                    request.MinPrice,
                    request.MaxPrice,
                    featureFilters,
                    request.PageNumber,
                    request.PageSize);

                // Get user's favorite base item IDs if user is authenticated
                var favoriteBaseItemIds = new HashSet<Guid>();
                if (request.UserId != Guid.Empty)
                {
                    var favorites = await _favoriteRepo.GetFavoritesByUserIdAsync(request.UserId);
                    favoriteBaseItemIds = favorites.Select(f => f.BaseItemId).ToHashSet();
                }

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

                _logger.LogInformation("Successfully retrieved {Count} products with advanced filters", productDTOs.Count);

                return Result<PaginatedResult<ProductDTO>>.Ok(
                    data: paginatedResult,
                    message: "تم جلب المنتجات بالفلاتر المتقدمة بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products with advanced filters");
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "فشل في جلب المنتجات بالفلاتر المتقدمة",
                    errorType: "GetProductsByAdvancedFiltersFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 