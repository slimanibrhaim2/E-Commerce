using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Commands.DeleteFromFavorite;

public class DeleteFromFavoriteCommandHandler : IRequestHandler<DeleteFromFavoriteCommand, Result<bool>>
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IProductRepository _productRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteFromFavoriteCommandHandler> _logger;

    public DeleteFromFavoriteCommandHandler(
        IFavoriteRepository favoriteRepository,
        IProductRepository productRepository,
        IServiceRepository serviceRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteFromFavoriteCommandHandler> logger)
    {
        _favoriteRepository = favoriteRepository;
        _productRepository = productRepository;
        _serviceRepository = serviceRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteFromFavoriteCommand request, CancellationToken cancellationToken)
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

            // Resolve BaseItemId to BaseItemId (similar to GetUserIdByItemIdQueryHandler)
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
                _logger.LogWarning(ex, "Error while checking product repository for item ID {BaseItemId}", request.ItemId);
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
                    _logger.LogWarning(ex, "Error while checking service repository for item ID {BaseItemId}", request.ItemId);
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

            // Find the favorite
            var favorites = await _favoriteRepository.FindAsync(f => 
                f.UserId == request.UserId && f.BaseItemId == baseItemId);
            
            var favorite = favorites.FirstOrDefault();
            if (favorite == null)
            {
                return Result<bool>.Fail(
                    message: "العنصر غير موجود في المفضلة",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            _favoriteRepository.Remove(favorite);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("تم حذف العنصر {BaseItemId} (BaseItemId: {BaseItemId}) من مفضلة المستخدم {UserId}", 
                request.ItemId, baseItemId, request.UserId);

            return Result<bool>.Ok(
                data: true,
                message: "تم حذف العنصر من المفضلة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل في حذف العنصر من المفضلة للمستخدم {UserId}", request.UserId);
            return Result<bool>.Fail(
                message: $"فشل في حذف العنصر من المفضلة: {ex.Message}",
                errorType: "DeleteFromFavoriteFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 