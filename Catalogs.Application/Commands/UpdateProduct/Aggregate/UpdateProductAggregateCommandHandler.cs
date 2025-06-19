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

namespace Catalogs.Application.Commands.UpdateProduct.Aggregate;

public class UpdateProductAggregateCommandHandler : IRequestHandler<UpdateProductAggregateCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly IFeatureRepository _featureRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBaseItemRepository _baseItemRepository;

    public UpdateProductAggregateCommandHandler(
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

    public async Task<Result<bool>> Handle(UpdateProductAggregateCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransaction();
        try
        {
            var dto = request.Product;
            if (request.Id == Guid.Empty)
                return Result<bool>.Fail("معرف المنتج مطلوب", "ValidationError", ResultStatus.ValidationError);
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<bool>.Fail("اسم المنتج مطلوب", "ValidationError", ResultStatus.ValidationError);
            if (dto.Price <= 0)
                return Result<bool>.Fail("سعر المنتج يجب أن يكون أكبر من الصفر", "ValidationError", ResultStatus.ValidationError);
            if (dto.CategoryId == Guid.Empty)
                return Result<bool>.Fail("معرف التصنيف مطلوب", "ValidationError", ResultStatus.ValidationError);

            var product = await _productRepository.GetById(request.Id);
            if (product == null)
                return Result<bool>.Fail("المنتج غير موجود", "NotFound", ResultStatus.NotFound);

            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return Result<bool>.Fail("التصنيف غير موجود", "ValidationError", ResultStatus.ValidationError);

            // Update BaseItem fields
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;
            product.IsAvailable = dto.IsAvailable;
            product.UserId = request.UserId;

            // Update Product fields
            product.SKU = dto.SKU;
            product.StockQuantity = dto.StockQuantity;

            await _productRepository.UpdateAsync(request.Id, product);
            await _unitOfWork.SaveChangesAsync();

            // Optionally: Remove all old media/features and add new ones (or sync)
            // This is a simple replace approach:
            // Remove all media
            var oldMedia = await _mediaRepository.GetMediaByItemIdAsync(request.Id);
            foreach (var media in oldMedia)
                await _mediaRepository.DeleteMediaAsync(media.Id);
            // Add new media
            foreach (var media in dto.Media ?? Enumerable.Empty<CreateMediaDTO>())
                await _mediaRepository.AddMediaAsync(request.Id, media.Url, media.MediaTypeId);

            // Remove all features
            // (Assume _featureRepository has a method to get and delete by productId)
            // Add new features
            foreach (var feature in dto.Features ?? Enumerable.Empty<CreateFeatureDTO>())
                await _featureRepository.AddFeatureAsync(request.Id, feature.Name, feature.Value);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransaction();
            return Result<bool>.Ok(true, "تم تحديث المنتج بنجاح", ResultStatus.Success);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransaction();
            return Result<bool>.Fail($"فشل في تحديث المنتج: {ex.Message}", "UpdateProductAggregateFailed", ResultStatus.Failed, ex);
        }
    }
}