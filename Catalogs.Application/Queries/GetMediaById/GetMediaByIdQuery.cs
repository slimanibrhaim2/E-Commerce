using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Queries.GetMediaById;

public record GetMediaByIdQuery(Guid Id) : IRequest<Result<MediaDTO>>; 