using MediatR;
using Core.Result;

namespace Catalogs.Application.Commands.DeleteFeature
{
    public record DeleteFeatureCommand(Guid Id) : IRequest<Result<bool>>;
} 