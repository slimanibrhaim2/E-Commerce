using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Core.Pagination;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Queries.GetMyFavorites;

public class GetMyFavoritesQueryHandler : IRequestHandler<GetMyFavoritesQuery, Result<PaginatedResult<FavoriteDTO>>>
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly ILogger<GetMyFavoritesQueryHandler> _logger;

    public GetMyFavoritesQueryHandler(
        IFavoriteRepository favoriteRepository,
        ILogger<GetMyFavoritesQueryHandler> logger)
    {
        _favoriteRepository = favoriteRepository;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<FavoriteDTO>>> Handle(GetMyFavoritesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validation
            if (request.UserId == Guid.Empty)
            {
                return Result<PaginatedResult<FavoriteDTO>>.Fail(
                    message: "معرف المستخدم مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageNumber < 1)
            {
                return Result<PaginatedResult<FavoriteDTO>>.Fail(
                    message: "رقم الصفحة يجب أن يكون أكبر من أو يساوي 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageSize < 1)
            {
                return Result<PaginatedResult<FavoriteDTO>>.Fail(
                    message: "حجم الصفحة يجب أن يكون أكبر من أو يساوي 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            // Get user's favorites
            var favorites = await _favoriteRepository.GetFavoritesByUserIdAsync(request.UserId);
            var favoritesList = favorites.ToList();

            // Apply pagination
            var totalCount = favoritesList.Count;
            var pagedFavorites = favoritesList
                .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                .Take(request.Parameters.PageSize)
                .ToList();

            // Map to DTOs
            var favoriteDTOs = pagedFavorites.Select(f => new FavoriteDTO
            {
                Id = f.Id,
                UserId = f.UserId,
                BaseItemId = f.BaseItemId,
                BaseItem = f.BaseItem != null ? new BaseItemDTO
                {
                    Id = f.BaseItem.Id,
                    Name = f.BaseItem.Name,
                    Description = f.BaseItem.Description,
                    Price = f.BaseItem.Price,
                    IsAvailable = f.BaseItem.IsAvailable,
                    CategoryId = f.BaseItem.CategoryId,
                    UserId = f.BaseItem.UserId
                } : null!,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt,
                DeletedAt = f.DeletedAt
            }).ToList();

            var paginatedResult = PaginatedResult<FavoriteDTO>.Create(
                data: favoriteDTOs,
                pageNumber: request.Parameters.PageNumber,
                pageSize: request.Parameters.PageSize,
                totalCount: totalCount
            );

            _logger.LogInformation("تم استرداد {Count} عنصر من مفضلة المستخدم {UserId}", 
                favoriteDTOs.Count, request.UserId);

            return Result<PaginatedResult<FavoriteDTO>>.Ok(
                data: paginatedResult,
                message: "تم استرداد المفضلة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل في استرداد مفضلة المستخدم {UserId}", request.UserId);
            return Result<PaginatedResult<FavoriteDTO>>.Fail(
                message: $"فشل في استرداد المفضلة: {ex.Message}",
                errorType: "GetMyFavoritesFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
}