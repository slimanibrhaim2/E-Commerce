using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Core.Pagination;
using System.Data;

namespace Catalogs.Application.Queries.GetProductsByCategory;

public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, Result<PaginatedResult<ProductDTO>>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetProductsByCategoryQueryHandler> _logger;
    private readonly IFeatureRepository _featureRepo;
    private readonly IFavoriteRepository _favoriteRepo;

    public GetProductsByCategoryQueryHandler(
        IProductRepository repo, 
        ILogger<GetProductsByCategoryQueryHandler> logger,
        IFeatureRepository featureRepo,
        IFavoriteRepository favoriteRepo)
    {
        _repo = repo;
        _logger = logger;
        _featureRepo = featureRepo;
        _favoriteRepo = favoriteRepo;
    }

    public async Task<Result<PaginatedResult<ProductDTO>>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.CategoryId == Guid.Empty)
            {
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "معرف الفئة مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.PageNumber < 1)
            {
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "Page number must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.PageSize < 1)
            {
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "Page size must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var products = await _repo.GetByCategory(request.CategoryId);
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
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
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
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                totalCount: totalCount
            );

            _logger.LogInformation("تم جلب {Count} منتج من الفئة {CategoryId}", productDTOs.Count, request.CategoryId);

            return Result<PaginatedResult<ProductDTO>>.Ok(
                data: paginatedResult,
                message: "تم جلب المنتجات بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving products by category");
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: "Failed to retrieve products due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل في جلب المنتجات من الفئة {CategoryId}", request.CategoryId);
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: $"فشل في جلب المنتجات: {ex.Message}",
                errorType: "GetProductsByCategoryFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 