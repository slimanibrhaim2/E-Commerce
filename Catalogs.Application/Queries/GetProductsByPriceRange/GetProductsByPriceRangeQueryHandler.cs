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

namespace Catalogs.Application.Queries.GetProductsByPriceRange;

public class GetProductsByPriceRangeQueryHandler : IRequestHandler<GetProductsByPriceRangeQuery, Result<PaginatedResult<ProductDTO>>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetProductsByPriceRangeQueryHandler> _logger;

    public GetProductsByPriceRangeQueryHandler(
        IProductRepository repo,
        ILogger<GetProductsByPriceRangeQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ProductDTO>>> Handle(GetProductsByPriceRangeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.MinPrice < 0)
            {
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "Minimum price cannot be negative",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.MaxPrice < 0)
            {
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "Maximum price cannot be negative",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.MaxPrice < request.MinPrice)
            {
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "Maximum price must be greater than or equal to minimum price",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

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

            var paginatedProducts = await _repo.GetByPriceRange(
                request.MinPrice,
                request.MaxPrice,
                request.PageNumber,
                request.PageSize);

            if (paginatedProducts.TotalCount == 0)
            {
                return Result<PaginatedResult<ProductDTO>>.Ok(
                    data: PaginatedResult<ProductDTO>.Create(
                        data: new List<ProductDTO>(),
                        pageNumber: request.PageNumber,
                        pageSize: request.PageSize,
                        totalCount: 0),
                    message: $"No products found with price between {request.MinPrice} and {request.MaxPrice}",
                    resultStatus: ResultStatus.Success);
            }

            var dtos = paginatedProducts.Data.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                SKU = p.SKU,
                StockQuantity = p.StockQuantity,
                IsAvailable = p.IsAvailable,
                UserId = p.UserId,
                Media = p.Media?.Select(m => new MediaDTO
                {
                    Url = m.MediaUrl,
                    MediaTypeName = m.MediaType?.Name
                }).ToList() ?? new List<MediaDTO>(),
                Features = p.Features?.Select(f => new ProductFeatureDTO
                {
                    Name = f.Name,
                    Value = f.Value
                }).ToList() ?? new List<ProductFeatureDTO>()
            }).ToList();

            var paginated = PaginatedResult<ProductDTO>.Create(
                data: dtos,
                pageNumber: paginatedProducts.PageNumber,
                pageSize: paginatedProducts.PageSize,
                totalCount: paginatedProducts.TotalCount);

            return Result<PaginatedResult<ProductDTO>>.Ok(
                data: paginated,
                message: $"Successfully retrieved {dtos.Count} products with price between {request.MinPrice} and {request.MaxPrice} out of {paginatedProducts.TotalCount} total products",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving products in price range {MinPrice} - {MaxPrice}", request.MinPrice, request.MaxPrice);
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: "Failed to retrieve products due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving products in price range {MinPrice} - {MaxPrice}: {Message}", 
                request.MinPrice, request.MaxPrice, ex.Message);
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: "An unexpected error occurred while retrieving products. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 