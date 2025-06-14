using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Catalogs.Application.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDTO>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(IProductRepository repo, ILogger<GetProductByIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<ProductDTO>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Id == Guid.Empty)
            {
                return Result<ProductDTO>.Fail(
                    message: "Product ID is required",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var product = await _repo.GetById(request.Id);
            if (product == null)
            {
                return Result<ProductDTO>.Fail(
                    message: $"Product with ID {request.Id} not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            var productDto = new ProductDTO
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
                Features = product.Features?.Select(f => new ProductFeatureDTO
                {
                    Id = f.Id,
                    Name = f.Name,
                    Value = f.Value,
                    ProductId = product.Id,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt
                }).ToList() ?? new List<ProductFeatureDTO>()
            };

            return Result<ProductDTO>.Ok(
                data: productDto,
                message: $"Successfully retrieved product with ID {request.Id}",
                resultStatus: ResultStatus.Success);
        }
        catch (DBConcurrencyException ex)
        {
            _logger.LogError(ex, "Database error while retrieving product with ID {ProductId}", request.Id);
            return Result<ProductDTO>.Fail(
                message: "Failed to retrieve product due to a database error. Please try again later.",
                errorType: "DatabaseError",
                resultStatus: ResultStatus.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving product with ID {ProductId}: {Message}", request.Id, ex.Message);
            return Result<ProductDTO>.Fail(
                message: "An unexpected error occurred while retrieving the product. Please try again later.",
                errorType: "UnexpectedError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 