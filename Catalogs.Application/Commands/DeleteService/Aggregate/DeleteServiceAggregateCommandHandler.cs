using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Catalogs.Application.Commands.DeleteService.Aggregate;

public class DeleteServiceAggregateCommandHandler : IRequestHandler<DeleteServiceAggregateCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IServiceRepository _serviceRepository;
    private readonly IMediaRepository _mediaRepository;
    private readonly IFeatureRepository _featureRepository;
    private readonly IBaseItemRepository _baseItemRepository;

    public DeleteServiceAggregateCommandHandler(
        IUnitOfWork unitOfWork,
        IServiceRepository serviceRepository,
        IMediaRepository mediaRepository,
        IFeatureRepository featureRepository,
        IBaseItemRepository baseItemRepository)
    {
        _unitOfWork = unitOfWork;
        _serviceRepository = serviceRepository;
        _mediaRepository = mediaRepository;
        _featureRepository = featureRepository;
        _baseItemRepository = baseItemRepository;
    }

    public async Task<Result<bool>> Handle(DeleteServiceAggregateCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransaction();
        try
        {
            if (request.Id == Guid.Empty)
                return Result<bool>.Fail("معرف الخدمة مطلوب", "ValidationError", ResultStatus.ValidationError);

            var service = await _serviceRepository.GetById(request.Id);
            if (service == null)
                return Result<bool>.Fail("الخدمة غير موجودة", "NotFound", ResultStatus.NotFound);

            // Delete all media
            var mediaList = await _mediaRepository.GetMediaByItemIdAsync(request.Id);
            foreach (var media in mediaList)
                await _mediaRepository.DeleteMediaAsync(media.Id);

            // Delete all features
            // (Assume _featureRepository has a method to get and delete by serviceId)

            // Delete service
            _serviceRepository.Remove(service);

            // Delete base item
            // (Assume service has a BaseItemId or similar linkage)
            // _baseItemRepository.RemoveById(service.BaseItemId); // Uncomment if such method exists

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransaction();
            return Result<bool>.Ok(true, "تم حذف الخدمة بنجاح", ResultStatus.Success);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransaction();
            return Result<bool>.Fail($"فشل في حذف الخدمة: {ex.Message}", "DeleteServiceAggregateFailed", ResultStatus.Failed, ex);
        }
    }
}