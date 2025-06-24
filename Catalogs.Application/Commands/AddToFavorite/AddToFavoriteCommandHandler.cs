using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Commands.AddToFavorite;

public class AddToFavoriteCommandHandler : IRequestHandler<AddToFavoriteCommand, Result<Guid>>
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IProductRepository _productRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddToFavoriteCommandHandler> _logger;

    public AddToFavoriteCommandHandler(
        IFavoriteRepository favoriteRepository,
        IProductRepository productRepository,
        IServiceRepository serviceRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddToFavoriteCommandHandler> logger)
    {
        _favoriteRepository = favoriteRepository;
        _productRepository = productRepository;
        _serviceRepository = serviceRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(AddToFavoriteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validation
            if (request.UserId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: "معرف المستخدم مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Favorite.ItemId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: "معرف العنصر مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            // Resolve ItemId to BaseItemId (similar to GetUserIdByItemIdQueryHandler)
            Guid baseItemId = Guid.Empty;
            
            // First, try to find as a product
            try
            {
                var product = await _productRepository.GetById(request.Favorite.ItemId);
                if (product != null)
                {
                    baseItemId = product.BaseItemId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while checking product repository for item ID {ItemId}", request.Favorite.ItemId);
            }

            // If not found as product, try to find as a service
            if (baseItemId == Guid.Empty)
            {
                try
                {
                    var service = await _serviceRepository.GetById(request.Favorite.ItemId);
                    if (service != null)
                    {
                        baseItemId = service.BaseItemId;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error while checking service repository for item ID {ItemId}", request.Favorite.ItemId);
                }
            }

            // If not found in either repository
            if (baseItemId == Guid.Empty)
            {
                return Result<Guid>.Fail(
                    message: $"لم يتم العثور على عنصر بالمعرف {request.Favorite.ItemId}",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // Check if already favorited
            var alreadyFavorited = await _favoriteRepository.IsFavoriteAsync(request.UserId, baseItemId);
            
            if (alreadyFavorited)
            {
                return Result<Guid>.Fail(
                    message: "العنصر موجود بالفعل في المفضلة",
                    errorType: "AlreadyExists",
                    resultStatus: ResultStatus.ValidationError);
            }

            var favorite = new Favorite
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                BaseItemId = baseItemId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _favoriteRepository.AddAsync(favorite);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("تم إضافة العنصر {ItemId} (BaseItemId: {BaseItemId}) إلى مفضلة المستخدم {UserId}", 
                request.Favorite.ItemId, baseItemId, request.UserId);

            return Result<Guid>.Ok(
                data: favorite.Id,
                message: "تم إضافة العنصر إلى المفضلة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل في إضافة العنصر إلى المفضلة للمستخدم {UserId}", request.UserId);
            return Result<Guid>.Fail(
                message: $"فشل في إضافة العنصر إلى المفضلة: {ex.Message}",
                errorType: "AddToFavoriteFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
}