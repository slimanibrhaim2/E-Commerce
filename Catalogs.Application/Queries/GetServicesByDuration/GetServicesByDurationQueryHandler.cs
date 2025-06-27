using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Core.Pagination;

namespace Catalogs.Application.Queries.GetServicesByDuration;

public class GetServicesByDurationQueryHandler : IRequestHandler<GetServicesByDurationQuery, Result<PaginatedResult<ServiceDTO>>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<GetServicesByDurationQueryHandler> _logger;
    private readonly IFeatureRepository _featureRepo;
    private readonly IFavoriteRepository _favoriteRepo;

    public GetServicesByDurationQueryHandler(
        IServiceRepository repo, 
        ILogger<GetServicesByDurationQueryHandler> logger,
        IFeatureRepository featureRepo,
        IFavoriteRepository favoriteRepo)
    {
        _repo = repo;
        _logger = logger;
        _featureRepo = featureRepo;
        _favoriteRepo = favoriteRepo;
    }

    public async Task<Result<PaginatedResult<ServiceDTO>>> Handle(GetServicesByDurationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.MinDuration > request.MaxDuration)
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "المدة الأدنى يجب أن تكون أقل من المدة الأقصى",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);

            var services = await _repo.GetByDurationRange(request.MinDuration, request.MaxDuration);
            var servicesList = services.ToList();

            // Get all favorites for the user if authenticated
            var userFavorites = request.UserId != Guid.Empty 
                ? await _favoriteRepo.GetFavoritesByUserIdAsync(request.UserId)
                : new List<Domain.Entities.Favorite>();

            // Get all favorite base item IDs for quick lookup
            var favoriteBaseItemIds = userFavorites.Select(f => f.BaseItemId).ToHashSet();

            var totalCount = servicesList.Count;
            var pageNumber = request.PageNumber;
            var pageSize = request.PageSize;
            var paged = servicesList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var serviceDTOs = new List<ServiceDTO>();
            foreach (var service in paged)
            {
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
                    IsFavorite = favoriteBaseItemIds.Contains(service.BaseItemId)
                };
                serviceDTOs.Add(serviceDTO);
            }

            var paginatedResult = PaginatedResult<ServiceDTO>.Create(
                data: serviceDTOs,
                pageNumber: pageNumber,
                pageSize: pageSize,
                totalCount: totalCount);

            return Result<PaginatedResult<ServiceDTO>>.Ok(
                data: paginatedResult,
                message: "تم جلب الخدمات بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving services in duration range {MinDuration} - {MaxDuration}", request.MinDuration, request.MaxDuration);
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "حدث خطأ أثناء جلب الخدمات",
                errorType: "GetServicesByDurationFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 