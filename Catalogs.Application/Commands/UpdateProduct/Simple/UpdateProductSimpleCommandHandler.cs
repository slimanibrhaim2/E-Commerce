using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Catalogs.Domain.Entities;

namespace Catalogs.Application.Commands.UpdateProduct.Simple;

public class UpdateProductSimpleCommandHandler : IRequestHandler<UpdateProductSimpleCommand, Result<bool>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<UpdateProductSimpleCommandHandler> _logger;

    public UpdateProductSimpleCommandHandler(IProductRepository repo, ILogger<UpdateProductSimpleCommandHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(UpdateProductSimpleCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.ProductDto.Name))
        {
            return Result<bool>.Fail(
                message: "اسم المنتج مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }
        if (request.ProductDto.Price <= 0)
        {
            return Result<bool>.Fail(
                message: "سعر المنتج يجب أن يكون أكبر من الصفر",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }
        if (request.ProductDto.CategoryId == Guid.Empty)
        {
            return Result<bool>.Fail(
                message: "معرف التصنيف مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            var product = new Product
            {
                Id = request.Id,
                Name = request.ProductDto.Name,
                Description = request.ProductDto.Description,
                Price = request.ProductDto.Price,
                CategoryId = request.ProductDto.CategoryId,
                SKU = request.ProductDto.SKU,
                StockQuantity = request.ProductDto.StockQuantity,
                IsAvailable = request.ProductDto.IsAvailable,
                // Add other properties as needed
            };

            var result = await _repo.UpdateAsync(request.Id, product);
            if (!result)
            {
                return Result<bool>.Fail(
                    message: "فشل في تحديث المنتج",
                    errorType: "UpdateProductFailed",
                    resultStatus: ResultStatus.Failed);
            }

            return Result<bool>.Ok(
                data: true,
                message: "تم تحديث المنتج بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product with ID {ProductId}", request.Id);
            return Result<bool>.Fail(
                message: "حدث خطأ أثناء تحديث المنتج",
                errorType: "UpdateProductFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
}