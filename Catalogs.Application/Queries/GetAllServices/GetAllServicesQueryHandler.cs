using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Core.Pagination;
using System.Data;

namespace Catalogs.Application.Queries.GetAllServices;

public class GetAllServicesQueryHandler : IRequestHandler<GetAllServicesQuery, Result<PaginatedResult<ServiceDTO>>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<GetAllServicesQueryHandler> _logger;
    private readonly IFeatureRepository _featureRepo;
    private readonly IFavoriteRepository _favoriteRepo;

    public GetAllServicesQueryHandler(
        IServiceRepository repo, 
        ILogger<GetAllServicesQueryHandler> logger,
        IFeatureRepository featureRepo,
        IFavoriteRepository favoriteRepo)
    {
        _repo = repo;
        _logger = logger;
        _featureRepo = featureRepo;
        _favoriteRepo = favoriteRepo;
    }

    public async Task<Result<PaginatedResult<ServiceDTO>>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Pagination.PageNumber < 1)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "Page number must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Pagination.PageSize < 1)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "Page size must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var services = await _repo.GetAllAsync();
            var servicesList = services.ToList();

            // Get all favorites for the user if authenticated
            var userFavorites = request.UserId != Guid.Empty 
                ? await _favoriteRepo.GetFavoritesByUserIdAsync(request.UserId)
                : new List<Domain.Entities.Favorite>();

            // Get all favorite base item IDs for quick lookup
            var favoriteBaseItemIds = userFavorites.Select(f => f.BaseItemId).ToHashSet();

            // Apply pagination
            var totalCount = servicesList.Count;
            var pagedServices = servicesList
                .Skip((request.Pagination.PageNumber - 1) * request.Pagination.PageSize)
                .Take(request.Pagination.PageSize)
                .ToList();

            // Map to DTOs and include features
            var serviceDTOs = new List<ServiceDTO>();
            foreach (var service in pagedServices)
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
                pageNumber: request.Pagination.PageNumber,
                pageSize: request.Pagination.PageSize,
                totalCount: totalCount
            );

            _logger.LogInformation("تم جلب {Count} خدمة", serviceDTOs.Count);

            return Result<PaginatedResult<ServiceDTO>>.Ok(
                data: paginatedResult,
                message: "تم جلب الخدمات بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving services");
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "Failed to retrieve services due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل في جلب الخدمات");
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: $"فشل في جلب الخدمات: {ex.Message}",
                errorType: "GetAllServicesFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 