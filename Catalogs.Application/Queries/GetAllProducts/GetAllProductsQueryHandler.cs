using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Core.Pagination;
using System.Data;

namespace Catalogs.Application.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<PaginatedResult<ProductDTO>>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetAllProductsQueryHandler> _logger;
    private readonly IFeatureRepository _featureRepo;

    public GetAllProductsQueryHandler(
        IProductRepository repo, 
        ILogger<GetAllProductsQueryHandler> logger,
        IFeatureRepository featureRepo)
    {
        _repo = repo;
        _logger = logger;
        _featureRepo = featureRepo;
    }

    public async Task<Result<PaginatedResult<ProductDTO>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Pagination.PageNumber < 1)
            {
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "Page number must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Pagination.PageSize < 1)
            {
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "Page size must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var products = await _repo.GetAllAsync();
            if (!products.Any())
            {
                return Result<PaginatedResult<ProductDTO>>.Ok(
                    data: PaginatedResult<ProductDTO>.Create(
                        data: new List<ProductDTO>(),
                        pageNumber: request.Pagination.PageNumber,
                        pageSize: request.Pagination.PageSize,
                        totalCount: 0),
                    message: "No products found",
                    resultStatus: ResultStatus.Success);
            }

            var productDtos = new List<ProductDTO>();

            foreach (var product in products)
            {
                try
                {
                    var features = await _featureRepo.GetProductFeaturesByEntityIdAsync(product.Id);
                    productDtos.Add(new ProductDTO
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        CategoryId = product.CategoryId,
                        SKU = product.SKU,
                        StockQuantity = product.StockQuantity,
                        IsAvailable = product.IsAvailable,
                        UserId = product.UserId,
                        CreatedAt = product.CreatedAt,
                        UpdatedAt = product.UpdatedAt,
                        Media = product.Media?.Select(m => new MediaDTO
                        {
                            Id = m.Id,
                            Url = m.MediaUrl,
                            MediaTypeId = m.MediaTypeId,
                            ItemId = m.BaseItemId,
                            CreatedAt = m.CreatedAt,
                            UpdatedAt = m.UpdatedAt
                        }).ToList() ?? new List<MediaDTO>(),
                        Features = features.Select(f => new ProductFeatureDTO
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Value = f.Value,
                            ProductId = f.BaseItemId,
                            CreatedAt = f.CreatedAt,
                            UpdatedAt = f.UpdatedAt
                        }).ToList()
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to process product {ProductId}: {Message}", product.Id, ex.Message);
                    // Continue processing other products even if one fails
                }
            }

            var totalCount = productDtos.Count;
            var paginatedProducts = PaginatedResult<ProductDTO>.Create(
                data: productDtos,
                pageNumber: request.Pagination.PageNumber,
                pageSize: request.Pagination.PageSize,
                totalCount: totalCount);

            return Result<PaginatedResult<ProductDTO>>.Ok(
                data: paginatedProducts,
                message: $"Successfully retrieved {productDtos.Count} products out of {totalCount} total products",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving products");
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: "Failed to retrieve products due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving products: {Message}", ex.Message);
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: "An unexpected error occurred while retrieving products. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 