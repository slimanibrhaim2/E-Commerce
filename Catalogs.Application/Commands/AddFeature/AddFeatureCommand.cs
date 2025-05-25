using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.AddFeature
{
    public record AddFeatureCommand(Guid EntityId, CreateFeatureDTO Feature) : IRequest<Result<Guid>>;
} 