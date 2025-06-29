using MediatR;
using Core.Result;
using Microsoft.Extensions.Logging;
using Catalogs.Domain.Repositories;
using Shared.Contracts.Commands;

namespace Catalogs.Application.Commands.UpdateProductQuantity;

public class UpdateProductQuantityCommandHandler : IRequestHandler<UpdateProductQuantityCommand, Result<bool>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<UpdateProductQuantityCommandHandler> _logger;

    public UpdateProductQuantityCommandHandler(
        IProductRepository productRepository,
        ILogger<UpdateProductQuantityCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(UpdateProductQuantityCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetById(request.ProductId);
            if (product == null)
            {
                return Result<bool>.Fail(
                    message: "المنتج غير موجود",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // Update the quantity
            product.StockQuantity += request.QuantityChange;
            
            // Ensure quantity doesn't go below 0
            if (product.StockQuantity < 0)
            {
                return Result<bool>.Fail(
                    message: "لا يمكن أن تكون الكمية أقل من 0",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            // Update availability based on quantity
            product.IsAvailable = product.StockQuantity > 0;
            
            await _productRepository.UpdateAsync(product.Id, product);

            return Result<bool>.Ok(
                data: true,
                message: "تم تحديث كمية المنتج بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating product quantity for product {ProductId}", request.ProductId);
            return Result<bool>.Fail(
                message: "حدث خطأ أثناء تحديث كمية المنتج",
                errorType: "ServerError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 