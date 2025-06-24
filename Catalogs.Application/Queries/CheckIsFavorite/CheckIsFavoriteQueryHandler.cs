using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Queries.CheckIsFavorite;

public class CheckIsFavoriteQueryHandler : IRequestHandler<CheckIsFavoriteQuery, Result<bool>>
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IProductRepository _productRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly ILogger<CheckIsFavoriteQueryHandler> _logger;

    public CheckIsFavoriteQueryHandler(
        IFavoriteRepository favoriteRepository,
        IProductRepository productRepository,
        IServiceRepository serviceRepository,
        ILogger<CheckIsFavoriteQueryHandler> logger)
    {
        _favoriteRepository = favoriteRepository;
        _productRepository = productRepository;
        _serviceRepository = serviceRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(CheckIsFavoriteQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validation
            if (request.UserId == Guid.Empty)
            {
                return Result<bool>.Fail(
                    message: "معرف المستخدم مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.ItemId == Guid.Empty)
            {
                return Result<bool>.Fail(
                    message: "معرف العنصر مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            // Resolve ItemId to BaseItemId (similar to other handlers)
            Guid baseItemId = Guid.Empty;
            
            // First, try to find as a product
            try
            {
                var product = await _productRepository.GetById(request.ItemId);
                if (product != null)
                {
                    baseItemId = product.BaseItemId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while checking product repository for item ID {ItemId}", request.ItemId);
            }

            // If not found as product, try to find as a service
            if (baseItemId == Guid.Empty)
            {
                try
                {
                    var service = await _serviceRepository.GetById(request.ItemId);
                    if (service != null)
                    {
                        baseItemId = service.BaseItemId;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error while checking service repository for item ID {ItemId}", request.ItemId);
                }
            }

            // If not found in either repository
            if (baseItemId == Guid.Empty)
            {
                return Result<bool>.Fail(
                    message: $"لم يتم العثور على عنصر بالمعرف {request.ItemId}",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // Check if favorited
            var isFavorited = await _favoriteRepository.IsFavoriteAsync(request.UserId, baseItemId);

            return Result<bool>.Ok(
                data: isFavorited,
                message: isFavorited ? "العنصر موجود في المفضلة" : "العنصر غير موجود في المفضلة",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل في فحص حالة المفضلة للعنصر {ItemId} والمستخدم {UserId}", request.ItemId, request.UserId);
            return Result<bool>.Fail(
                message: $"فشل في فحص حالة المفضلة: {ex.Message}",
                errorType: "CheckIsFavoriteFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 