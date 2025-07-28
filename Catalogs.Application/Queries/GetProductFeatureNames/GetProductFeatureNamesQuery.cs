using MediatR;
using Core.Result;

namespace Catalogs.Application.Queries.GetProductFeatureNames
{
    public record GetProductFeatureNamesQuery() : IRequest<Result<List<string>>>;
} 