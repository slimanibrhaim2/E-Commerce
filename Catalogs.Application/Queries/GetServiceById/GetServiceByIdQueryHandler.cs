using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Catalogs.Application.Queries.GetServiceById;

public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, Result<ServiceDTO>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<GetServiceByIdQueryHandler> _logger;
    private readonly IFeatureRepository _featureRepo;
    private readonly IFavoriteRepository _favoriteRepo;

    public GetServiceByIdQueryHandler(
        IServiceRepository repo,
        ILogger<GetServiceByIdQueryHandler> logger,
        IFeatureRepository featureRepo,
        IFavoriteRepository favoriteRepo)
    {
        _repo = repo;
        _logger = logger;
        _featureRepo = featureRepo;
        _favoriteRepo = favoriteRepo;
    }

    public async Task<Result<ServiceDTO>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Id == Guid.Empty)
            {
                return Result<ServiceDTO>.Fail(
                    message: "معرف الخدمة مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var service = await _repo.GetByIdAsync(request.Id);
            
            if (service == null)
            {
                return Result<ServiceDTO>.Fail(
                    message: $"الخدمة غير موجودة {request.Id}",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // Check if the service is in user's favorites
            var isFavorite = request.UserId != Guid.Empty && 
                await _favoriteRepo.GetFavoritesByUserIdAsync(request.UserId) is var favorites &&
                favorites.Any(f => f.BaseItemId == service.BaseItemId);

            var features = await _featureRepo.GetServiceFeaturesByEntityIdAsync(service.Id);
            var serviceDTO = new ServiceDTO
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                CategoryId = service.CategoryId,
                ServiceType = service.ServiceType,
                Duration = service.Duration,
                IsAvailable = service.IsAvailable,
                UserId = service.UserId,
                CreatedAt = service.CreatedAt,
                UpdatedAt = service.UpdatedAt,
                Media = service.Media?.Select(m => new MediaDTO
                {
                    Id = m.Id,
                    Url = m.MediaUrl,
                    MediaTypeId = m.MediaTypeId,
                    BaseItemId = m.BaseItemId
                }).ToList() ?? new List<MediaDTO>(),
                Features = features.Select(f => new ServiceFeatureDTO
                {
                    Id = f.Id,
                    Name = f.Name,
                    Value = f.Value
                }).ToList(),
                IsFavorite = isFavorite
            };

            return Result<ServiceDTO>.Ok(
                data: serviceDTO,
                message: "تم جلب الخدمة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving service with ID {ServiceId}", request.Id);
            return Result<ServiceDTO>.Fail(
                message: "Failed to retrieve service due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل في جلب الخدمة {ServiceId}", request.Id);
            return Result<ServiceDTO>.Fail(
                message: $"فشل في جلب الخدمة: {ex.Message}",
                errorType: "GetServiceByIdFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 