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
    private readonly IFavoriteRepository _favoriteRepo;

    public GetServicesByDurationQueryHandler(
        IServiceRepository repo, 
        ILogger<GetServicesByDurationQueryHandler> logger,
        IFavoriteRepository favoriteRepo)
    {
        _repo = repo;
        _logger = logger;
        _favoriteRepo = favoriteRepo;
    }

    public async Task<Result<PaginatedResult<ServiceDTO>>> Handle(GetServicesByDurationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.MinDuration > request.MaxDuration)
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "Minimum duration must be less than maximum duration",
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

            var serviceDTOs = paged.Select(service => new ServiceDTO
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                CategoryName = service.Category?.Name,
                ServiceType = service.ServiceType,
                Duration = service.Duration,
                IsAvailable = service.IsAvailable,
                Media = service.Media?.Select(m => new MediaDTO
                {
                    Url = m.MediaUrl,
                    MediaTypeName = m.MediaType?.Name
                }).ToList() ?? new List<MediaDTO>(),
                Features = service.Features?.Select(f => new ServiceFeatureDTO
                {
                    Name = f.Name,
                    Value = f.Value
                }).ToList() ?? new List<ServiceFeatureDTO>(),
                IsFavorite = favoriteBaseItemIds.Contains(service.BaseItemId)
            }).ToList();

            var paginatedResult = PaginatedResult<ServiceDTO>.Create(
                data: serviceDTOs,
                pageNumber: pageNumber,
                pageSize: pageSize,
                totalCount: totalCount);

            return Result<PaginatedResult<ServiceDTO>>.Ok(
                data: paginatedResult,
                message: "Services retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving services in duration range {MinDuration} - {MaxDuration}", request.MinDuration, request.MaxDuration);
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "An error occurred while retrieving services",
                errorType: "GetServicesByDurationFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 