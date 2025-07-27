using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.UpdateService;

public record UpdateServiceAggregateCommand(Guid Id, CreateServiceAggregateDTO Service, Guid UserId) : IRequest<Result<bool>>;