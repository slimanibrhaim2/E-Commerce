using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.CreateMediaType;

public record CreateMediaTypeCommand(CreateMediaTypeDTO MediaType) : IRequest<Result<Guid>>; 