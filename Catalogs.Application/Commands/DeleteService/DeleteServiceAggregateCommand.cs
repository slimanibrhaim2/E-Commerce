using MediatR;
using Core.Result;

namespace Catalogs.Application.Commands.DeleteService;

public record DeleteServiceAggregateCommand(Guid Id) : IRequest<Result<bool>>;