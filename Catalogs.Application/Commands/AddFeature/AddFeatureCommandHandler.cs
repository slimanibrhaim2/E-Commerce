using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Core.Interfaces;

namespace Catalogs.Application.Commands.AddFeature;

public class AddFeatureCommandHandler : IRequestHandler<AddFeatureCommand, Result<Guid>>
{
    private readonly IFeatureRepository _featureRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddFeatureCommandHandler(IFeatureRepository featureRepository, IUnitOfWork unitOfWork)
    {
        _featureRepository = featureRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(AddFeatureCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (request.EntityId == Guid.Empty)
        {
            return Result<Guid>.Fail(
                message: "معرف الكيان مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }
        if (string.IsNullOrWhiteSpace(request.Feature.Name))
        {
            return Result<Guid>.Fail(
                message: "اسم الميزة مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }
        if (string.IsNullOrWhiteSpace(request.Feature.Value))
        {
            return Result<Guid>.Fail(
                message: "قيمة الميزة مطلوبة",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            // Try to add the feature (product or service)
            var featureId = await _featureRepository.AddFeatureAsync(
                request.EntityId,
                request.Feature.Name,
                request.Feature.Value);

            if (featureId == null)
            {
                return Result<Guid>.Fail(message: "Entity not found", errorType: "FeatureNotFound", resultStatus: ResultStatus.NotFound);
            }

            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Ok(data: featureId.Value,
                message: "تم إضافة الميزة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Fail(
             message: $"Failed to add feature: {ex.Message}",
             errorType: "AddFeatureFailed",
             resultStatus: ResultStatus.Failed,
             exception: ex
            );
        }
    }
} 