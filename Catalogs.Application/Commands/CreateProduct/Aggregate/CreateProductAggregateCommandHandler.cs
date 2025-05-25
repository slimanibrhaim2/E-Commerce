using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Catalogs.Application.Commands.CreateProduct.Aggregate;

public class CreateProductAggregateCommandHandler : IRequestHandler<CreateProductAggregateCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly IFeatureRepository _featureRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBaseItemRepository _baseItemRepository;

    public CreateProductAggregateCommandHandler(
        IUnitOfWork unitOfWork,
        IProductRepository productRepository,
        IMediaRepository mediaRepository,
        IFeatureRepository featureRepository,
        ICategoryRepository categoryRepository,
        IBaseItemRepository baseItemRepository)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _mediaRepository = mediaRepository;
        _featureRepository = featureRepository;
        _categoryRepository = categoryRepository;
        _baseItemRepository = baseItemRepository;
    }

    public async Task<Result<Guid>> Handle(CreateProductAggregateCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransaction();
        try
        {
            // 1. Validate product
            var dto = request.Product;
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<Guid>.Fail("اسم المنتج مطلوب", "ValidationError", ResultStatus.ValidationError);
            if (dto.Price <= 0)
                return Result<Guid>.Fail("سعر المنتج يجب أن يكون أكبر من الصفر", "ValidationError", ResultStatus.ValidationError);
            if (dto.CategoryId == Guid.Empty)
                return Result<Guid>.Fail("معرف التصنيف مطلوب", "ValidationError", ResultStatus.ValidationError);

            // 2. Check category existence
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return Result<Guid>.Fail("التصنيف غير موجود", "ValidationError", ResultStatus.ValidationError);

            // 2. Create and persist BaseItem first
            var baseItem = new BaseItem
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                UserId = dto.UserId, // Ensure UserId is provided in the DTO
                IsAvailable = dto.IsAvailable
            };
            await _baseItemRepository.AddAsync(baseItem);
            await _unitOfWork.SaveChangesAsync();
            if (baseItem == null || baseItem.CategoryId == Guid.Empty)
                throw new Exception("فشل في إنشاء العنصر الأساسي");

            // 3. Create product with BaseItemId as FK
            var product = new Product
            {
                Id = Guid.NewGuid(),
                BaseItemId= baseItem.Id,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                SKU = dto.SKU,
                StockQuantity = dto.StockQuantity,
                IsAvailable = dto.IsAvailable,
                UserId = dto.UserId,
            };
            // If Product has a BaseItemId property, set it here. If not, ensure mapping is correct in the repo/mapper.
            // product.BaseItemId = baseItem.Id; // Uncomment if such property exists
            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            if (product.Id == Guid.Empty)
                throw new Exception("فشل في إنشاء المنتج");

            // 4. Add media
            foreach (var media in dto.Media ?? Enumerable.Empty<CreateMediaDTO>())
            {
                await _mediaRepository.AddMediaAsync(product.Id, media.Url, media.MediaTypeId);
            }
            // 5. Add features
            foreach (var feature in dto.Features ?? Enumerable.Empty<CreateFeatureDTO>())
            {
                await _featureRepository.AddFeatureAsync(product.Id, feature.Name, feature.Value);
            }
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransaction();
            return Result<Guid>.Ok(product.Id, "تم إنشاء المنتج مع الوسائط والميزات بنجاح", ResultStatus.Success);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransaction();
            return Result<Guid>.Fail($"فشل في إنشاء المنتج: {ex.Message}", "CreateProductAggregateFailed", ResultStatus.Failed, ex);
        }
    }
} 