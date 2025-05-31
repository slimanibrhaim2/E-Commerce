using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Core.Interfaces;

namespace Catalogs.Application.Commands.UpdateFeature;

public class UpdateFeatureCommandHandler : IRequestHandler<UpdateFeatureCommand, Result<bool>>
{
    private readonly IFeatureRepository _featureRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFeatureCommandHandler(IFeatureRepository featureRepository, IUnitOfWork unitOfWork)
    {
        _featureRepository = featureRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateFeatureCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Feature.Name))
        {
            return Result<bool>.Fail(
                message: "اسم الميزة مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }
        if (string.IsNullOrWhiteSpace(request.Feature.Value))
        {
            return Result<bool>.Fail(
                message: "قيمة الميزة مطلوبة",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            var updated = await _featureRepository.UpdateFeatureAsync(
                request.Id,
                request.Feature.Name,
                request.Feature.Value);
            await _unitOfWork.SaveChangesAsync();

            if (!updated)
            {
                return Result<bool>.Fail(
                    message: "لم يتم العثور على الميزة أو فشل التحديث",
                    errorType: "FeatureNotFoundOrUpdateFailed",
                    resultStatus: ResultStatus.NotFound);
            }

            return Result<bool>.Ok(
                data: true,
                message: "تم تحديث الميزة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"فشل في تحديث الميزة: {ex.Message}",
                errorType: "UpdateFeatureFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 