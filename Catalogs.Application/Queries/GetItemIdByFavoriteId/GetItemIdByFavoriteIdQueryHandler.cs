using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Catalogs.Application.Queries.GetItemIdByFavoriteId;

public class GetItemIdByFavoriteIdQueryHandler : IRequestHandler<GetItemIdByFavoriteIdQuery, Result<ItemIdResponseDTO>>
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IProductRepository _productRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly ILogger<GetItemIdByFavoriteIdQueryHandler> _logger;

    public GetItemIdByFavoriteIdQueryHandler(
        IFavoriteRepository favoriteRepository,
        IProductRepository productRepository,
        IServiceRepository serviceRepository,
        ILogger<GetItemIdByFavoriteIdQueryHandler> logger)
    {
        _favoriteRepository = favoriteRepository;
        _productRepository = productRepository;
        _serviceRepository = serviceRepository;
        _logger = logger;
    }

    public async Task<Result<ItemIdResponseDTO>> Handle(GetItemIdByFavoriteIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.FavoriteId == Guid.Empty)
            {
                return Result<ItemIdResponseDTO>.Fail(
                    message: "معرف المفضلة مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            // Get the favorite to find the BaseItemId
            var favorite = await _favoriteRepository.GetByIdAsync(request.FavoriteId);
            if (favorite == null)
            {
                return Result<ItemIdResponseDTO>.Fail(
                    message: $"لم يتم العثور على مفضلة بالمعرف {request.FavoriteId}",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // Now resolve BaseItemId back to the original BaseItemId (Product or Service)
            // First, try to find as a product
            try
            {
                var products = await _productRepository.FindAsync(p => p.BaseItemId == favorite.BaseItemId);
                var product = products.FirstOrDefault();
                if (product != null)
                {
                    var response = new ItemIdResponseDTO
                    {
                        ItemId = product.Id,
                        ItemType = "Product"
                    };

                    return Result<ItemIdResponseDTO>.Ok(
                        data: response,
                        message: $"تم العثور على معرف المنتج بنجاح",
                        resultStatus: ResultStatus.Success);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while checking product repository for BaseItemId {BaseItemId}", favorite.BaseItemId);
            }

            // If not found as product, try to find as a service
            try
            {
                var services = await _serviceRepository.FindAsync(s => s.BaseItemId == favorite.BaseItemId);
                var service = services.FirstOrDefault();
                if (service != null)
                {
                    var response = new ItemIdResponseDTO
                    {
                        ItemId = service.Id,
                        ItemType = "Service"
                    };

                    return Result<ItemIdResponseDTO>.Ok(
                        data: response,
                        message: $"تم العثور على معرف الخدمة بنجاح",
                        resultStatus: ResultStatus.Success);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while checking service repository for BaseItemId {BaseItemId}", favorite.BaseItemId);
            }

            // If not found in either repository
            return Result<ItemIdResponseDTO>.Fail(
                message: $"لم يتم العثور على عنصر مرتبط بالمفضلة {request.FavoriteId}",
                errorType: "NotFound",
                resultStatus: ResultStatus.NotFound);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving item ID for favorite with ID {FavoriteId}", request.FavoriteId);
            return Result<ItemIdResponseDTO>.Fail(
                message: "فشل في جلب معرف العنصر بسبب خطأ في قاعدة البيانات. يرجى المحاولة مرة أخرى لاحقاً.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving item ID for favorite with ID {FavoriteId}: {Message}", request.FavoriteId, ex.Message);
            return Result<ItemIdResponseDTO>.Fail(
                message: "حدث خطأ غير متوقع أثناء جلب معرف العنصر. يرجى المحاولة مرة أخرى لاحقاً.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 