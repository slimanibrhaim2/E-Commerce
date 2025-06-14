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

namespace Catalogs.Application.Queries.GetProductsByName;

public class GetProductsByNameQueryHandler : IRequestHandler<GetProductsByNameQuery, Result<PaginatedResult<ProductDTO>>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetProductsByNameQueryHandler> _logger;

    public GetProductsByNameQueryHandler(
        IProductRepository repo,
        ILogger<GetProductsByNameQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ProductDTO>>> Handle(GetProductsByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "Product name cannot be empty",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageNumber < 1)
            {
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "Page number must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.Parameters.PageSize < 1)
            {
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "Page size must be greater than or equal to 1",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var products = (await _repo.GetProductsByNameAsync(request.Name)).ToList();
            
            if (!products.Any())
            {
                return Result<PaginatedResult<ProductDTO>>.Ok(
                    data: PaginatedResult<ProductDTO>.Create(
                        data: new List<ProductDTO>(),
                        pageNumber: request.Parameters.PageNumber,
                        pageSize: request.Parameters.PageSize,
                        totalCount: 0),
                    message: $"No products found matching the name '{request.Name}'",
                    resultStatus: ResultStatus.Success);
            }

            var totalCount = products.Count;
            var pageNumber = request.Parameters.PageNumber;
            var pageSize = request.Parameters.PageSize;
            var paged = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            
            var dtos = paged.Select(p => new ProductDTO
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
                Media = p.Media?.Select(m => new MediaDTO
                {
                    Id = m.Id,
                    Url = m.MediaUrl,
                    MediaTypeId = m.MediaTypeId,
                    ItemId = m.BaseItemId,
                    MediaType = m.MediaType != null ? new MediaTypeDTO
                    {
                        Id = m.MediaType.Id,
                        Name = m.MediaType.Name,
                        CreatedAt = m.MediaType.CreatedAt,
                        UpdatedAt = m.MediaType.UpdatedAt
                    } : null,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt
                }).ToList() ?? new List<MediaDTO>(),
                Features = p.Features?.Select(f => new ProductFeatureDTO
                {
                    Id = f.Id,
                    Name = f.Name,
                    Value = f.Value,
                    ProductId = p.Id,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                }).ToList() ?? new List<ProductFeatureDTO>()
            }).ToList();

            var paginated = PaginatedResult<ProductDTO>.Create(dtos, pageNumber, pageSize, totalCount);
            return Result<PaginatedResult<ProductDTO>>.Ok(
                data: paginated,
                message: $"Successfully retrieved {dtos.Count} products matching the name '{request.Name}'",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while searching for products with name '{ProductName}'", request.Name);
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: "Failed to search products due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while searching for products with name '{ProductName}': {Message}", request.Name, ex.Message);
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: "An unexpected error occurred while searching for products. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 