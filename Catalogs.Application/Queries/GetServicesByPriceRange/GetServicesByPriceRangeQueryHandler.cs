using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Core.Pagination;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Catalogs.Application.Queries.GetServicesByPriceRange;

public class GetServicesByPriceRangeQueryHandler : IRequestHandler<GetServicesByPriceRangeQuery, Result<PaginatedResult<ServiceDTO>>>
{
    private readonly IServiceRepository _repo;
    private readonly ILogger<GetServicesByPriceRangeQueryHandler> _logger;
    private readonly IFavoriteRepository _favoriteRepo;

    public GetServicesByPriceRangeQueryHandler(
        IServiceRepository repo,
        ILogger<GetServicesByPriceRangeQueryHandler> logger,
        IFavoriteRepository favoriteRepo)
    {
        _repo = repo;
        _logger = logger;
        _favoriteRepo = favoriteRepo;
    }

    public async Task<Result<PaginatedResult<ServiceDTO>>> Handle(GetServicesByPriceRangeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.MinPrice < 0)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "Minimum price cannot be negative",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.MaxPrice < 0)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "Maximum price cannot be negative",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.MaxPrice < request.MinPrice)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "Maximum price must be greater than or equal to minimum price",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.PageNumber < 1)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "Page number must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.PageSize < 1)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "Page size must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var services = (await _repo.GetByPriceRange(request.MinPrice, request.MaxPrice)).ToList();
            
            // Get all favorites for the user if authenticated
            var userFavorites = request.UserId != Guid.Empty 
                ? await _favoriteRepo.GetFavoritesByUserIdAsync(request.UserId)
                : new List<Domain.Entities.Favorite>();

            // Get all favorite base item IDs for quick lookup
            var favoriteBaseItemIds = userFavorites.Select(f => f.BaseItemId).ToHashSet();

            if (!services.Any())
            {
                return Result<PaginatedResult<ServiceDTO>>.Ok(
                    data: PaginatedResult<ServiceDTO>.Create(
                        data: new List<ServiceDTO>(),
                        pageNumber: request.PageNumber,
                        pageSize: request.PageSize,
                        totalCount: 0),
                    message: $"No services found with price between {request.MinPrice} and {request.MaxPrice}",
                    resultStatus: ResultStatus.Success);
            }

            var totalCount = services.Count;
            var pageNumber = request.PageNumber;
            var pageSize = request.PageSize;
            var paged = services.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            
            var dtos = paged.Select(s => new ServiceDTO
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                CategoryName = s.Category?.Name,
                ServiceType = s.ServiceType,
                Duration = s.Duration,
                IsAvailable = s.IsAvailable,
                Media = s.Media?.Select(m => new MediaDTO
                {
                    Url = m.MediaUrl,
                    MediaTypeName = m.MediaType?.Name
                }).ToList() ?? new List<MediaDTO>(),
                Features = s.Features?.Select(f => new ServiceFeatureDTO
                {
                    Name = f.Name,
                    Value = f.Value
                }).ToList() ?? new List<ServiceFeatureDTO>(),
                IsFavorite = favoriteBaseItemIds.Contains(s.BaseItemId)
            }).ToList();

            var paginated = PaginatedResult<ServiceDTO>.Create(
                data: dtos,
                pageNumber: pageNumber,
                pageSize: pageSize,
                totalCount: totalCount);

            return Result<PaginatedResult<ServiceDTO>>.Ok(
                data: paginated,
                message: $"Successfully retrieved {dtos.Count} services with price between {request.MinPrice} and {request.MaxPrice}",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving services in price range {MinPrice} - {MaxPrice}", request.MinPrice, request.MaxPrice);
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "An error occurred while retrieving services",
                errorType: "GetServicesByPriceRangeFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 