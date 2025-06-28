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

namespace Catalogs.Application.Queries.GetLowStockProducts;

public class GetLowStockProductsQueryHandler : IRequestHandler<GetLowStockProductsQuery, Result<PaginatedResult<ProductDTO>>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetLowStockProductsQueryHandler> _logger;

    public GetLowStockProductsQueryHandler(
        IProductRepository repo,
        ILogger<GetLowStockProductsQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ProductDTO>>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Threshold < 0)
            {
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "Threshold must be greater than or equal to 0",
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

            var products = await _repo.GetLowStockProducts(request.Threshold);
            var productList = products.ToList();
            var totalCount = productList.Count;

            if (!productList.Any())
            {
                return Result<PaginatedResult<ProductDTO>>.Ok(
                    data: PaginatedResult<ProductDTO>.Create(
                        data: new List<ProductDTO>(),
                        pageNumber: request.PageNumber,
                        pageSize: request.PageSize,
                        totalCount: 0),
                    message: $"No products found with stock quantity below {request.Threshold}",
                    resultStatus: ResultStatus.Success);
            }

            var pagedProducts = productList
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var productDtos = pagedProducts.Select(p => new ProductDTO
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

            var paginatedProducts = PaginatedResult<ProductDTO>.Create(
                data: productDtos,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                totalCount: totalCount);

            return Result<PaginatedResult<ProductDTO>>.Ok(
                data: paginatedProducts,
                message: $"Successfully retrieved {productDtos.Count} low stock products out of {totalCount} total products",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving low stock products");
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: "Failed to retrieve products due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving low stock products: {Message}", ex.Message);
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: "An unexpected error occurred while retrieving products. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 