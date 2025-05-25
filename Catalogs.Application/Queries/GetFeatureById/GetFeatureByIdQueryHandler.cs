using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Queries.GetFeatureById;

public class GetFeatureByIdQueryHandler : IRequestHandler<GetFeatureByIdQuery, Result<FeatureDTO>>
{
    private readonly IFeatureRepository _featureRepository;
    private readonly ILogger<GetFeatureByIdQueryHandler> _logger;

    public GetFeatureByIdQueryHandler(
        IFeatureRepository featureRepository,
        ILogger<GetFeatureByIdQueryHandler> logger)
    {
        _featureRepository = featureRepository;
        _logger = logger;
    }

    public async Task<Result<FeatureDTO>> Handle(GetFeatureByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var productFeature = await _featureRepository.GetProductFeatureByIdAsync(request.Id);
            if (productFeature != null)
            {
                var dto = new FeatureDTO
                {
                    Id = productFeature.Id,
                    Name = productFeature.Name,
                    Value = productFeature.Value,
                    EntityId = productFeature.BaseItemId
                };
                return Result<FeatureDTO>.Ok(
                    data: dto,
                    message: "تم جلب الميزة بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            var serviceFeature = await _featureRepository.GetServiceFeatureByIdAsync(request.Id);
            if (serviceFeature != null)
            {
                var dto = new FeatureDTO
                {
                    Id = serviceFeature.Id,
                    Name = serviceFeature.Name,
                    Value = serviceFeature.Value,
                    EntityId = serviceFeature.ServiceId
                };
                return Result<FeatureDTO>.Ok(
                    data: dto,
                    message: "تم جلب الميزة بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            return Result<FeatureDTO>.Fail(
                message: "الميزة غير موجودة",
                errorType: "NotFound",
                resultStatus: ResultStatus.NotFound);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting feature by ID");
            return Result<FeatureDTO>.Fail(
                message: "فشل في جلب الميزة",
                errorType: "InternalServerError",
                resultStatus: ResultStatus.InternalServerError,
                exception: ex);
        }
    }
} 