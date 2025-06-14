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

namespace Catalogs.Application.Commands.CreateService.Aggregate;

public class CreateServiceAggregateCommandHandler : IRequestHandler<CreateServiceAggregateCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IServiceRepository _serviceRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly IFeatureRepository _featureRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBaseItemRepository _baseItemRepository;
    private readonly ICouponRepository _couponRepository;

    public CreateServiceAggregateCommandHandler(
        IUnitOfWork unitOfWork,
        IServiceRepository serviceRepository,
        IMediaRepository mediaRepository,
        IFeatureRepository featureRepository,
        ICategoryRepository categoryRepository,
        IBaseItemRepository baseItemRepository,
        ICouponRepository couponRepository)
    {
        _unitOfWork = unitOfWork;
        _serviceRepository = serviceRepository;
        _mediaRepository = mediaRepository;
        _featureRepository = featureRepository;
        _categoryRepository = categoryRepository;
        _baseItemRepository = baseItemRepository;
        _couponRepository = couponRepository;
    }

    public async Task<Result<Guid>> Handle(CreateServiceAggregateCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransaction();
        try
        {
            // 1. Validate service
            var dto = request.Service;
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<Guid>.Fail("اسم الخدمة مطلوب", "ValidationError", ResultStatus.ValidationError);
            if (dto.Price <= 0)
                return Result<Guid>.Fail("سعر الخدمة يجب أن يكون أكبر من الصفر", "ValidationError", ResultStatus.ValidationError);
            if (dto.CategoryId == Guid.Empty)
                return Result<Guid>.Fail("معرف التصنيف مطلوب", "ValidationError", ResultStatus.ValidationError);
            if (string.IsNullOrWhiteSpace(dto.ServiceType))
                return Result<Guid>.Fail("نوع الخدمة مطلوب", "ValidationError", ResultStatus.ValidationError);
            if (dto.Duration <= 0)
                return Result<Guid>.Fail("مدة الخدمة يجب أن تكون أكبر من الصفر", "ValidationError", ResultStatus.ValidationError);

            // 2. Check category existence
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return Result<Guid>.Fail("التصنيف غير موجود", "ValidationError", ResultStatus.ValidationError);

            // 3. Create and persist BaseItem first
            var baseItem = new BaseItem
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                UserId = dto.UserId,
                IsAvailable = dto.IsAvailable
            };
            await _baseItemRepository.AddAsync(baseItem);
            await _unitOfWork.SaveChangesAsync();
            if (baseItem == null || baseItem.CategoryId == Guid.Empty)
                throw new Exception("فشل في إنشاء العنصر الأساسي");

            // 4. Create service with BaseItemId as FK
            var service = new Service
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                ServiceType = dto.ServiceType,
                Duration = dto.Duration,
                IsAvailable = dto.IsAvailable,
                UserId = dto.UserId,
                BaseItemId = baseItem.Id
            };
            await _serviceRepository.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();
            if (service.Id == Guid.Empty)
                throw new Exception("فشل في إنشاء الخدمة");

            // 5. Add media
            foreach (var media in dto.Media ?? Enumerable.Empty<CreateMediaDTO>())
            {
                await _mediaRepository.AddMediaAsync(service.Id, media.Url, media.MediaTypeId);
            }

            // 6. Add features
            foreach (var feature in dto.Features ?? Enumerable.Empty<CreateFeatureDTO>())
            {
                await _featureRepository.AddFeatureAsync(service.Id, feature.Name, feature.Value);
            }

            // 7. Add coupons
            foreach (var coupon in dto.Coupons ?? Enumerable.Empty<CreateCouponDTO>())
            {
                var couponEntity = new Coupon
                {
                    Id = Guid.NewGuid(),
                    UserId = dto.UserId,
                    Code = coupon.Code,
                    DiscountAmount = coupon.DiscountAmount,
                    ExpiryDate = coupon.ExpiryDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _couponRepository.AddAsync(couponEntity);
                service.ApplicableCoupons ??= new List<Coupon>();
                service.ApplicableCoupons.Add(couponEntity);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransaction();
            return Result<Guid>.Ok(service.Id, "تم إنشاء الخدمة مع الوسائط والميزات والكوبونات بنجاح", ResultStatus.Success);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransaction();
            return Result<Guid>.Fail($"فشل في إنشاء الخدمة: {ex.Message}", "CreateServiceAggregateFailed", ResultStatus.Failed, ex);
        }
    }
} 