using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Core.Pagination;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Queries.GetMyFavorites;

public class GetMyFavoritesQueryHandler : IRequestHandler<GetMyFavoritesQuery, Result<PaginatedResult<FavoriteResponseDTO>>>
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IProductRepository _productRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly ILogger<GetMyFavoritesQueryHandler> _logger;

    public GetMyFavoritesQueryHandler(
        IFavoriteRepository favoriteRepository,
        IProductRepository productRepository,
        IServiceRepository serviceRepository,
        ILogger<GetMyFavoritesQueryHandler> logger)
    {
        _favoriteRepository = favoriteRepository;
        _productRepository = productRepository;
        _serviceRepository = serviceRepository;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<FavoriteResponseDTO>>> Handle(GetMyFavoritesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Validation
            if (request.UserId == Guid.Empty)
            {
                return Result<PaginatedResult<FavoriteResponseDTO>>.Fail(
                    message: "معرف المستخدم مطلوب",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageNumber < 1)
            {
                return Result<PaginatedResult<FavoriteResponseDTO>>.Fail(
                    message: "رقم الصفحة يجب أن يكون أكبر من أو يساوي 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageSize < 1)
            {
                return Result<PaginatedResult<FavoriteResponseDTO>>.Fail(
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
            var favoriteDTOs = new List<FavoriteResponseDTO>();
            
            foreach (var f in pagedFavorites)
            {
                double quantity = 0;

                // Try to get product quantity
                var productId = await _productRepository.GetProductIdByBaseItemIdAsync(f.BaseItemId);
                if (productId.HasValue)
                {
                    var product = await _productRepository.GetById(productId.Value);
                    if (product != null)
                    {
                        quantity = product.StockQuantity;
                    }
                }
                else
                {
                    // If not a product, it's a service (quantity = 1)
                    quantity = 1;
                }

                if (f.BaseItem != null)
                {
                    favoriteDTOs.Add(new FavoriteResponseDTO
                    {
                        Quantity = quantity,
                        BaseItem = new BaseItemResponseDTO
                        {
                            Name = f.BaseItem.Name,
                            Description = f.BaseItem.Description,
                            Price = f.BaseItem.Price,
                            IsAvailable = f.BaseItem.IsAvailable
                        },
                        CreatedAt = f.CreatedAt,
                        UpdatedAt = f.UpdatedAt
                    });
                }
            }

            var paginatedResult = PaginatedResult<FavoriteResponseDTO>.Create(
                data: favoriteDTOs,
                pageNumber: request.Parameters.PageNumber,
                pageSize: request.Parameters.PageSize,
                totalCount: totalCount
            );

            _logger.LogInformation("تم استرداد {Count} عنصر من مفضلة المستخدم {UserId}", 
                favoriteDTOs.Count, request.UserId);

            return Result<PaginatedResult<FavoriteResponseDTO>>.Ok(
                data: paginatedResult,
                message: "تم استرداد المفضلة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "فشل في استرداد مفضلة المستخدم {UserId}", request.UserId);
            return Result<PaginatedResult<FavoriteResponseDTO>>.Fail(
                message: $"فشل في استرداد المفضلة: {ex.Message}",
                errorType: "GetMyFavoritesFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
}