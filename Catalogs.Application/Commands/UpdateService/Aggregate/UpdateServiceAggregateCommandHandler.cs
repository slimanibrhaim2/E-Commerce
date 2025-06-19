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

namespace Catalogs.Application.Commands.UpdateService.Aggregate;

public class UpdateServiceAggregateCommandHandler : IRequestHandler<UpdateServiceAggregateCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IServiceRepository _serviceRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly IFeatureRepository _featureRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBaseItemRepository _baseItemRepository;

    public UpdateServiceAggregateCommandHandler(
        IUnitOfWork unitOfWork,
        IServiceRepository serviceRepository,
        IMediaRepository mediaRepository,
        IFeatureRepository featureRepository,
        ICategoryRepository categoryRepository,
        IBaseItemRepository baseItemRepository)
    {
        _unitOfWork = unitOfWork;
        _serviceRepository = serviceRepository;
        _mediaRepository = mediaRepository;
        _featureRepository = featureRepository;
        _categoryRepository = categoryRepository;
        _baseItemRepository = baseItemRepository;
    }

    public async Task<Result<bool>> Handle(UpdateServiceAggregateCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransaction();
        try
        {
            var dto = request.Service;
            if (request.Id == Guid.Empty)
                return Result<bool>.Fail("معرف الخدمة مطلوب", "ValidationError", ResultStatus.ValidationError);
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<bool>.Fail("اسم الخدمة مطلوب", "ValidationError", ResultStatus.ValidationError);
            if (dto.Price <= 0)
                return Result<bool>.Fail("سعر الخدمة يجب أن يكون أكبر من الصفر", "ValidationError", ResultStatus.ValidationError);
            if (dto.CategoryId == Guid.Empty)
                return Result<bool>.Fail("معرف التصنيف مطلوب", "ValidationError", ResultStatus.ValidationError);
            if (string.IsNullOrWhiteSpace(dto.ServiceType))
                return Result<bool>.Fail("نوع الخدمة مطلوب", "ValidationError", ResultStatus.ValidationError);
            if (dto.Duration <= 0)
                return Result<bool>.Fail("مدة الخدمة يجب أن تكون أكبر من الصفر", "ValidationError", ResultStatus.ValidationError);

            var service = await _serviceRepository.GetById(request.Id);
            if (service == null)
                return Result<bool>.Fail("الخدمة غير موجودة", "NotFound", ResultStatus.NotFound);

            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return Result<bool>.Fail("التصنيف غير موجود", "ValidationError", ResultStatus.ValidationError);

            // Update BaseItem fields
            service.Name = dto.Name;
            service.Description = dto.Description;
            service.Price = dto.Price;
            service.CategoryId = dto.CategoryId;
            service.IsAvailable = dto.IsAvailable;
            service.UserId = request.UserId;

            // Update Service fields
            service.ServiceType = dto.ServiceType;
            service.Duration = dto.Duration;

            await _serviceRepository.UpdateAsync(request.Id, service);
            await _unitOfWork.SaveChangesAsync();

            // Optionally: Remove all old media/features and add new ones (or sync)
            // Remove all media
            var oldMedia = await _mediaRepository.GetMediaByItemIdAsync(request.Id);
            foreach (var media in oldMedia)
                await _mediaRepository.DeleteMediaAsync(media.Id);
            // Add new media
            foreach (var media in dto.Media ?? Enumerable.Empty<CreateMediaDTO>())
                await _mediaRepository.AddMediaAsync(request.Id, media.Url, media.MediaTypeId);

            // Remove all features
            // (Assume _featureRepository has a method to get and delete by serviceId)
            // Add new features
            foreach (var feature in dto.Features ?? Enumerable.Empty<CreateFeatureDTO>())
                await _featureRepository.AddFeatureAsync(request.Id, feature.Name, feature.Value);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransaction();
            return Result<bool>.Ok(true, "تم تحديث الخدمة بنجاح", ResultStatus.Success);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransaction();
            return Result<bool>.Fail($"فشل في تحديث الخدمة: {ex.Message}", "UpdateServiceAggregateFailed", ResultStatus.Failed, ex);
        }
    }
}