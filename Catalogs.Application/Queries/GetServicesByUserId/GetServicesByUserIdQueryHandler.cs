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

namespace Catalogs.Application.Queries.GetServicesByUserId;

public class GetServicesByUserIdQueryHandler : IRequestHandler<GetServicesByUserIdQuery, Result<PaginatedResult<ServiceDTO>>>
{
    private readonly IServiceRepository _repository;
    private readonly ILogger<GetServicesByUserIdQueryHandler> _logger;
    private readonly IFavoriteRepository _favoriteRepo;

    public GetServicesByUserIdQueryHandler(
        IServiceRepository repository,
        ILogger<GetServicesByUserIdQueryHandler> logger,
        IFavoriteRepository favoriteRepo)
    {
        _repository = repository;
        _logger = logger;
        _favoriteRepo = favoriteRepo;
    }

    public async Task<Result<PaginatedResult<ServiceDTO>>> Handle(GetServicesByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.UserId == Guid.Empty)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "معرف المستخدم مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageNumber < 1)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "يجب أن يكون رقم الصفحة أكبر من أو يساوي 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageSize < 1)
            {
                return Result<PaginatedResult<ServiceDTO>>.Fail(
                    message: "يجب أن يكون حجم الصفحة أكبر من أو يساوي 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var paginatedServices = await _repository.GetServicesByUserIdAsync(request.UserId, request.Parameters.PageNumber, request.Parameters.PageSize);
            
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
                        pageNumber: request.Parameters.PageNumber,
                        pageSize: request.Parameters.PageSize,
                        totalCount: 0),
                    message: $"لم يتم العثور على خدمات للمستخدم {request.UserId}",
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
                pageNumber: request.Parameters.PageNumber,
                pageSize: request.Parameters.PageSize,
                totalCount: paginatedServices.TotalCount);

            return Result<PaginatedResult<ServiceDTO>>.Ok(
                data: paginated,
                message: $"تم جلب {dtos.Count} خدمة للمستخدم {request.UserId}",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب الخدمات للمستخدم {UserId}", request.UserId);
            return Result<PaginatedResult<ServiceDTO>>.Fail(
                message: "حدث خطأ أثناء جلب الخدمات",
                errorType: "GetServicesByUserIdFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 