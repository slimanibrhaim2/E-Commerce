using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Core.Interfaces;

namespace Catalogs.Application.Commands.CreateProduct.Simple;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<CreateProductCommandHandler> _logger;
    private readonly IBaseItemRepository _baseItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IProductRepository repo, ILogger<CreateProductCommandHandler> logger, IBaseItemRepository baseItemRepository, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _logger = logger;
        _baseItemRepository = baseItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.ProductDto.Name))
        {
            return Result<Guid>.Fail(
                message: "اسم المنتج مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }
        if (request.ProductDto.Price <= 0)
        {
            return Result<Guid>.Fail(
                message: "سعر المنتج يجب أن يكون أكبر من الصفر",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }
        if (request.ProductDto.CategoryId == Guid.Empty)
        {
            return Result<Guid>.Fail(
                message: "معرف التصنيف مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            // 1. Create and persist BaseItem first
            var baseItem = new BaseItem
            {
                Id = Guid.NewGuid(),
                Name = request.ProductDto.Name,
                Description = request.ProductDto.Description,
                Price = request.ProductDto.Price,
                CategoryId = request.ProductDto.CategoryId,
                UserId = request.UserId,
                IsAvailable = request.ProductDto.IsAvailable
            };
            await _baseItemRepository.AddAsync(baseItem);
            await _unitOfWork.SaveChangesAsync();

            // 2. Create product with BaseItemId as FK (if applicable)
            var product = new Product
            {
                Id = Guid.NewGuid(),
                BaseItemId = baseItem.Id,
                Name = request.ProductDto.Name,
                Description = request.ProductDto.Description,
                Price = request.ProductDto.Price,
                CategoryId = request.ProductDto.CategoryId,
                SKU = request.ProductDto.SKU,
                StockQuantity = request.ProductDto.StockQuantity,
                IsAvailable = request.ProductDto.IsAvailable,
                UserId = request.UserId,
                // Set the BaseItemId or link to the baseItem as needed
            };
            // If Product has a BaseItemId property, set it here. If not, ensure mapping is correct in the repo/mapper.
            // product.BaseItemId = baseItem.Id; // Uncomment if such property exists
            await _repo.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            if (product.Id == Guid.Empty)
                return Result<Guid>.Fail(
                    message: "فشل في إنشاء المنتج",
                    errorType: "CreateProductFailed",
                    resultStatus: ResultStatus.Failed);

            return Result<Guid>.Ok(
                data: product.Id,
                message: "تم إنشاء المنتج بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return Result<Guid>.Fail(
                message: $"فشل في إنشاء المنتج: {ex.Message}",
                errorType: "CreateProductFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 