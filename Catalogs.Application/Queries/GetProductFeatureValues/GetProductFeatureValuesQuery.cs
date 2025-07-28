using MediatR;
using Core.Result;

namespace Catalogs.Application.Queries.GetProductFeatureValues
{
    public record GetProductFeatureValuesQuery(string FeatureName) : IRequest<Result<List<string>>>;
} 