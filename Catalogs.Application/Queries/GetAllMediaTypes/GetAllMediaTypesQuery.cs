using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Queries.GetAllMediaTypes;

public record GetAllMediaTypesQuery() : IRequest<Result<List<MediaTypeDTO>>>; 