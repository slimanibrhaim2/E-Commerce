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

namespace Catalogs.Application.Queries.GetProductsByUserId;

public class GetProductsByUserIdQueryHandler : IRequestHandler<GetProductsByUserIdQuery, Result<PaginatedResult<ProductDTO>>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetProductsByUserIdQueryHandler> _logger;

    public GetProductsByUserIdQueryHandler(
        IProductRepository repo,
        ILogger<GetProductsByUserIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<ProductDTO>>> Handle(GetProductsByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.UserId == Guid.Empty)
            {
                return Result<PaginatedResult<ProductDTO>>.Fail(
                    message: "User ID is required",
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

            var products = (await _repo.GetProductsByUserIdAsync(request.UserId)).ToList();
            
            if (!products.Any())
            {
                return Result<PaginatedResult<ProductDTO>>.Ok(
                    data: PaginatedResult<ProductDTO>.Create(
                        data: new List<ProductDTO>(),
                        pageNumber: request.Parameters.PageNumber,
                        pageSize: request.Parameters.PageSize,
                        totalCount: 0),
                    message: $"No products found for user {request.UserId}",
                    resultStatus: ResultStatus.Success);
            }

            var totalCount = products.Count;
            var pageNumber = request.Parameters.PageNumber;
            var pageSize = request.Parameters.PageSize;
            var paged = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            
            var dtos = paged.Select(p => new ProductDTO
            {
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                SKU = p.SKU,
                StockQuantity = p.StockQuantity,
                IsAvailable = p.IsAvailable,
                CategoryName = p.Category?.Name,
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

            var paginated = PaginatedResult<ProductDTO>.Create(dtos, pageNumber, pageSize, totalCount);
            return Result<PaginatedResult<ProductDTO>>.Ok(
                data: paginated,
                message: $"Successfully retrieved {dtos.Count} products for user {request.UserId} out of {totalCount} total products",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving products for user {UserId}", request.UserId);
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: "Failed to retrieve products due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving products for user {UserId}: {Message}", request.UserId, ex.Message);
            return Result<PaginatedResult<ProductDTO>>.Fail(
                message: "An unexpected error occurred while retrieving products. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 