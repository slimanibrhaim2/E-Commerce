using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using Core.Pagination;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;

namespace Catalogs.Application.Queries.GetProductsByIds;

public class GetProductsByIdsQueryHandler : IRequestHandler<GetProductsByIdsQuery, Result<PaginatedResult<ProductDetailsDTO>>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetProductsByIdsQueryHandler> _logger;

    public GetProductsByIdsQueryHandler(
        IProductRepository repo,
        ILogger<GetProductsByIdsQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ProductDetailsDTO>>> Handle(GetProductsByIdsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.ProductIds == null || !request.ProductIds.Any())
            {
                return Result<PaginatedResult<ProductDetailsDTO>>.Fail(
                    message: "Product IDs list cannot be empty",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.ProductIds.Any(id => id == Guid.Empty))
            {
                return Result<PaginatedResult<ProductDetailsDTO>>.Fail(
                    message: "All product IDs must be valid GUIDs",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageNumber < 1)
            {
                return Result<PaginatedResult<ProductDetailsDTO>>.Fail(
                    message: "Page number must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageSize < 1)
            {
                return Result<PaginatedResult<ProductDetailsDTO>>.Fail(
                    message: "Page size must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var products = (await _repo.GetByIdsAsync(request.ProductIds)).ToList();
            
            if (!products.Any())
            {
                return Result<PaginatedResult<ProductDetailsDTO>>.Ok(
                    data: PaginatedResult<ProductDetailsDTO>.Create(
                        data: new List<ProductDetailsDTO>(),
                        pageNumber: request.Parameters.PageNumber,
                        pageSize: request.Parameters.PageSize,
                        totalCount: 0),
                    message: $"No products found for the provided IDs: {string.Join(", ", request.ProductIds)}",
                    resultStatus: ResultStatus.Success);
            }

            var totalCount = products.Count;
            var pageNumber = request.Parameters.PageNumber;
            var pageSize = request.Parameters.PageSize;
            var paged = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            
            var dtos = paged.Select(p => new ProductDetailsDTO
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
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Media = p.Media?.Select(m => new Shared.Contracts.DTOs.MediaDTO
                {
                    Id = m.Id,
                    Url = m.MediaUrl,
                    MediaTypeId = m.MediaTypeId,
                    ItemId = m.BaseItemId,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToList() ?? new List<Shared.Contracts.DTOs.MediaDTO>(),
                Features = p.Features?.Select(f => new Shared.Contracts.DTOs.ProductFeatureDTO
                {
                    Id = f.Id,
                    Name = f.Name,
                    Value = f.Value,
                    ProductId = p.Id,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                }).ToList() ?? new List<Shared.Contracts.DTOs.ProductFeatureDTO>()
            }).ToList();

            var paginated = PaginatedResult<ProductDetailsDTO>.Create(dtos, pageNumber, pageSize, totalCount);
            return Result<PaginatedResult<ProductDetailsDTO>>.Ok(
                data: paginated,
                message: $"Successfully retrieved {dtos.Count} products out of {request.ProductIds.Count()} requested IDs",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving products for IDs: {ProductIds}", string.Join(", ", request.ProductIds));
            return Result<PaginatedResult<ProductDetailsDTO>>.Fail(
                message: "Failed to retrieve products due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving products for IDs: {ProductIds}: {Message}", 
                string.Join(", ", request.ProductIds), ex.Message);
            return Result<PaginatedResult<ProductDetailsDTO>>.Fail(
                message: "An unexpected error occurred while retrieving products. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 