using MediatR;
using Core.Result;

namespace Catalogs.Application.Commands.DeleteService.Aggregate;

public record DeleteServiceAggregateCommand(Guid Id) : IRequest<Result<bool>>;