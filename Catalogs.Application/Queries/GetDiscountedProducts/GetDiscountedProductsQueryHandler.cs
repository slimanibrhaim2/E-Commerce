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

namespace Catalogs.Application.Queries.GetDiscountedProducts;

public class GetDiscountedProductsQueryHandler : IRequestHandler<GetDiscountedProductsQuery, Result<PaginatedResult<ProductDTO>>>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<GetDiscountedProductsQueryHandler> _logger;

    public GetDiscountedProductsQueryHandler(
        IProductRepository repository,
        ILogger<GetDiscountedProductsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ProductDTO>>> Handle(GetDiscountedProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PageNumber < 1)
            {
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "Page number must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.PageSize < 1)
            {
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "Page size must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var products = await _repository.GetDiscountedProducts();
            
            if (!products.Any())
            {
                return Result<PaginatedResult<ProductDTO>>.Ok(
                    data: PaginatedResult<ProductDTO>.Create(
                        data: new List<ProductDTO>(),
                        pageNumber: request.PageNumber,
                        pageSize: request.PageSize,
                        totalCount: 0),
                    message: "No discounted products found",
                    resultStatus: ResultStatus.Success);
            }
            var productList = products.ToList();
            var totalCount = productList.Count;
            var pageNumber = request.PageNumber;
            var pageSize = request.PageSize;
            var paged = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            
            var dtos = paged.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CategoryId = p.CategoryId,
                SKU = p.SKU,
                IsAvailable = p.IsAvailable,
                UserId = p.UserId,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Media = p.Media?.Select(m => new MediaDTO
                {
                    Id = m.Id,
                    Url = m.MediaUrl,
                    MediaTypeId = m.MediaTypeId,
                    ItemId = m.BaseItemId,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToList() ?? new List<MediaDTO>(),
                Features = p.Features?.Select(f => new ProductFeatureDTO
                {
                    Id = f.Id,
                    Name = f.Name,
                    Value = f.Value,
                    ProductId = f.BaseItemId,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                }).ToList() ?? new List<ProductFeatureDTO>()
            }).ToList();

            var paginated = PaginatedResult<ProductDTO>.Create(dtos, pageNumber, pageSize, totalCount);
            return Result<PaginatedResult<ProductDTO>>.Ok(
                data: paginated,
                message: $"Successfully retrieved {dtos.Count} discounted products out of {totalCount} total products",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving discounted products");
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: "Failed to retrieve products due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving discounted products: {Message}", ex.Message);
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: "An unexpected error occurred while retrieving products. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 