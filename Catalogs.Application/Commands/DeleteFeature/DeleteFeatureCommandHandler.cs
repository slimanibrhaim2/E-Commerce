using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;

namespace Catalogs.Application.Commands.DeleteFeature;

public class DeleteFeatureCommandHandler : IRequestHandler<DeleteFeatureCommand, Result<bool>>
{
    private readonly IFeatureRepository _featureRepository;

    public DeleteFeatureCommandHandler(IFeatureRepository featureRepository)
    {
        _featureRepository = featureRepository;
    }

    public async Task<Result<bool>> Handle(DeleteFeatureCommand request, CancellationToken cancellationToken)
    {
        // Validation
        if (request.Id == Guid.Empty)
        {
            return Result<bool>.Fail(
                message: "معرف الميزة مطلوب",
                errorType: "ValidationError",
                resultStatus: ResultStatus.ValidationError);
        }

        try
        {
            var productFeature = await _featureRepository.GetProductFeatureByIdAsync(request.Id);
            var serviceFeature = await _featureRepository.GetServiceFeatureByIdAsync(request.Id);
            if (productFeature == null && serviceFeature == null)
            {
                return Result<bool>.Fail(message: "Feature not found", 
                    errorType: "FeatureNotFound", 
                    resultStatus: ResultStatus.NotFound);
            }

            var success = await _featureRepository.DeleteFeatureAsync(request.Id);
            if (!success)
            {
                return Result<bool>.Fail(message: "Failed to delete feature", 
                    errorType: "DeleteFeatureFailed", 
                    resultStatus: ResultStatus.Failed);
            }

            return Result<bool>.Ok(data:true,
                message: "تم حذف الميزة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"Failed to delete feature: {ex.Message}",
                errorType: "DeleteFeatureFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex
            );
        }
    }
} 