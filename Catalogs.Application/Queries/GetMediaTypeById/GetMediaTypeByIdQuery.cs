using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Queries.GetMediaTypeById;

public record GetMediaTypeByIdQuery(Guid Id) : IRequest<Result<MediaTypeDTO>>; 