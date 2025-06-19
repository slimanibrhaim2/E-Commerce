using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.UpdateService.Simple;

public record UpdateServiceSimpleCommand(Guid Id, CreateServiceDTO Service, Guid UserId) : IRequest<Result<bool>>;