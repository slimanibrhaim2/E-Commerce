using MediatR;
using Core.Result;

namespace Catalogs.Application.Queries.GetProductFeatureNames
{
    public record GetProductFeatureNamesQuery(Guid? CategoryId) : IRequest<Result<List<string>>>;
} 