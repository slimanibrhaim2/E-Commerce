using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;

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
            var product = await _repo.GetById(request.Id);
            if (product == null)
                return Result<ProductDTO>.Fail(
                    message: "المنتج غير موجود",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);

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
                // Add other properties as needed
            };

            return Result<ProductDTO>.Ok(
                data: productDto,
                message: "تم جلب المنتج بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product by ID");
            return Result<ProductDTO>.Fail(
                message: "حدث خطأ أثناء جلب المنتج",
                errorType: "GetProductByIdFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 