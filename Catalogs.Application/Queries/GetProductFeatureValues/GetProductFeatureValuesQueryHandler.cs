using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Queries.GetProductFeatureValues
{
    public class GetProductFeatureValuesQueryHandler 
        : IRequestHandler<GetProductFeatureValuesQuery, Result<List<string>>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<GetProductFeatureValuesQueryHandler> _logger;

        public GetProductFeatureValuesQueryHandler(
            IProductRepository productRepository,
            ILogger<GetProductFeatureValuesQueryHandler> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<Result<List<string>>> Handle(
            GetProductFeatureValuesQuery request, 
            CancellationToken cancellationToken)
        {
            try
            {
                // Validate feature name
                if (string.IsNullOrEmpty(request.FeatureName?.Trim()))
                {
                    return Result<List<string>>.Fail(
                        message: "اسم الميزة مطلوب",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                _logger.LogInformation("Getting unique feature values for feature: {FeatureName}", request.FeatureName);

                var featureValues = await _productRepository.GetUniqueFeatureValuesByNameAsync(request.FeatureName);

                _logger.LogInformation("Successfully retrieved {Count} unique values for feature '{FeatureName}'", 
                    featureValues.Count, request.FeatureName);

                return Result<List<string>>.Ok(
                    data: featureValues,
                    message: "تم جلب قيم الميزة بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feature values for feature name: {FeatureName}", request.FeatureName);
                return Result<List<string>>.Fail(
                    message: "فشل في جلب قيم الميزة",
                    errorType: "GetProductFeatureValuesFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 