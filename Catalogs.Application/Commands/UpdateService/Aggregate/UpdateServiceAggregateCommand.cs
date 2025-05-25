using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.UpdateService.Aggregate;

public record UpdateServiceAggregateCommand(Guid Id, CreateServiceAggregateDTO Service) : IRequest<Result<bool>>;