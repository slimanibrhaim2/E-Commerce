using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.UpdateMediaType;

public record UpdateMediaTypeCommand(Guid Id, CreateMediaTypeDTO MediaType) : IRequest<Result<bool>>; 