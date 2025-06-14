using MediatR;

namespace Communication.Application.Commands.DeleteMessageAggregate;

public record DeleteMessageAggregateCommand(Guid Id) : IRequest<Core.Result.Result<bool>>; 