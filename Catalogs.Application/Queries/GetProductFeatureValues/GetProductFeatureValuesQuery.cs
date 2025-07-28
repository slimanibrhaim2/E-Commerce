using MediatR;
using Core.Result;

namespace Catalogs.Application.Queries.GetProductFeatureValues
{
    public record GetProductFeatureValuesQuery(string FeatureName, Guid? CategoryId) : IRequest<Result<List<string>>>;
} 