using MediatR;
using Core.Result;
using Catalogs.Application.DTOs;

namespace Catalogs.Application.Commands.CreateService.Aggregate;

public record CreateServiceAggregateCommand(CreateServiceAggregateDTO Service, Guid UserId) : IRequest<Result<Guid>>; 