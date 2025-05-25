using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Queries.GetFeatureById
{
    // Handler maps from domain entity to DTO; repository returns domain entities only.
    public record GetFeatureByIdQuery(Guid Id) : IRequest<Result<FeatureDTO>>;
} 