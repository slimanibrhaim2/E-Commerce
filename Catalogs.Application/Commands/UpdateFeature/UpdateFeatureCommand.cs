using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.UpdateFeature
{
    public record UpdateFeatureCommand(Guid Id, CreateFeatureDTO Feature) : IRequest<Result<bool>>;
} 