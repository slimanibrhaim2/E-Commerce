using MediatR;
using Core.Result;
using Catalogs.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Queries.GetProductFeatureNames
{
    public class GetProductFeatureNamesQueryHandler 
        : IRequestHandler<GetProductFeatureNamesQuery, Result<List<string>>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<GetProductFeatureNamesQueryHandler> _logger;

        public GetProductFeatureNamesQueryHandler(
            IProductRepository productRepository,
            ILogger<GetProductFeatureNamesQueryHandler> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<Result<List<string>>> Handle(
            GetProductFeatureNamesQuery request, 
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting unique product feature names for categoryId: {CategoryId}", request.CategoryId);

                var featureNames = await _productRepository.GetUniqueFeatureNamesAsync(request.CategoryId);

                _logger.LogInformation("Successfully retrieved {Count} unique feature names for categoryId: {CategoryId}", 
                    featureNames.Count, request.CategoryId);

                return Result<List<string>>.Ok(
                    data: featureNames,
                    message: "تم جلب أسماء الميزات بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product feature names for categoryId: {CategoryId}", request.CategoryId);
                return Result<List<string>>.Fail(
                    message: "فشل في جلب أسماء الميزات",
                    errorType: "GetProductFeatureNamesFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 