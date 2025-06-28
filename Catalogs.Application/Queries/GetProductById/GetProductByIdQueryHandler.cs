using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Catalogs.Application.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDTO>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;
    private readonly IFeatureRepository _featureRepo;
    private readonly IFavoriteRepository _favoriteRepo;

    public GetProductByIdQueryHandler(
        IProductRepository repo, 
        ILogger<GetProductByIdQueryHandler> logger,
        IFeatureRepository featureRepo,
        IFavoriteRepository favoriteRepo)
    {
        _repo = repo;
        _logger = logger;
        _featureRepo = featureRepo;
        _favoriteRepo = favoriteRepo;
    }

    public async Task<Result<ProductDTO>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Id == Guid.Empty)
            {
                return Result<ProductDTO>.Fail(
                    message: "معرف المنتج مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var product = await _repo.GetByIdWithDetails(request.Id);
            if (product == null)
            {
                return Result<ProductDTO>.Fail(
                    message: $"المنتج غير موجود {request.Id}",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // Check if the product is in user's favorites
            var isFavorite = request.UserId != Guid.Empty && 
                await _favoriteRepo.GetFavoritesByUserIdAsync(request.UserId) is var favorites &&
                favorites.Any(f => f.BaseItemId == product.BaseItemId);

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
                    Url = m.MediaUrl,
                    MediaTypeName = m.MediaType?.Name ?? "unknown"
                }).ToList() ?? new List<MediaDTO>(),
                Features = features.Select(f => new ProductFeatureDTO
                {
                    Name = f.Name,
                    Value = f.Value
                }).ToList(),
                IsFavorite = isFavorite
            };

            return Result<ProductDTO>.Ok(
                data: productDTO,
                message: "تم جلب المنتج بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل في جلب المنتج {ProductId}", request.Id);
            return Result<ProductDTO>.Fail(
                message: $"فشل في جلب المنتج: {ex.Message}",
                errorType: "GetProductByIdFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 