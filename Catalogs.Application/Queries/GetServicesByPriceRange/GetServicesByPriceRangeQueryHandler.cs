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
                    message: "لا يمكن أن يكون السعر الأدنى سالباً",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.MaxPrice < 0)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "لا يمكن أن يكون السعر الأقصى سالباً",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.MaxPrice < request.MinPrice)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "يجب أن يكون السعر الأقصى أكبر من أو يساوي السعر الأدنى",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.PageNumber < 1)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "يجب أن يكون رقم الصفحة أكبر من أو يساوي 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.PageSize < 1)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "يجب أن يكون حجم الصفحة أكبر من أو يساوي 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var paginatedServices = await _repo.GetByPriceRange(request.MinPrice, request.MaxPrice, request.PageNumber, request.PageSize);
            
            // Get all favorites for the user if authenticated
            var userFavorites = request.UserId != Guid.Empty 
                ? await _favoriteRepo.GetFavoritesByUserIdAsync(request.UserId)
                : new List<Domain.Entities.Favorite>();

            // Get all favorite base item IDs for quick lookup
            var favoriteBaseItemIds = userFavorites.Select(f => f.BaseItemId).ToHashSet();

            if (!paginatedServices.Data.Any())
            {
                return Result<PaginatedResult<ServiceDTO>>.Ok(
                    data: PaginatedResult<ServiceDTO>.Create(
                        data: new List<ServiceDTO>(),
                        pageNumber: request.PageNumber,
                        pageSize: request.PageSize,
                        totalCount: 0),
                    message: $"لم يتم العثور على خدمات بسعر بين {request.MinPrice} و {request.MaxPrice}",
                    resultStatus: ResultStatus.Success);
            }
            
            var dtos = paginatedServices.Data.Select(s => new ServiceDTO
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                CategoryName = s.Category?.Name,
                ServiceType = s.ServiceType,
                Duration = s.Duration,
                IsAvailable = s.IsAvailable,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
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
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                totalCount: paginatedServices.TotalCount);

            return Result<PaginatedResult<ServiceDTO>>.Ok(
                data: paginated,
                message: $"تم جلب {dtos.Count} خدمة بسعر بين {request.MinPrice} و {request.MaxPrice}",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب الخدمات في نطاق السعر {MinPrice} - {MaxPrice}", request.MinPrice, request.MaxPrice);
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "حدث خطأ أثناء جلب الخدمات",
                errorType: "GetServicesByPriceRangeFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 