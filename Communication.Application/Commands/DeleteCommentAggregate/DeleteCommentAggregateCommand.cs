using MediatR;

namespace Communication.Application.Commands.DeleteCommentAggregate;

public record DeleteCommentAggregateCommand(Guid Id) : IRequest<Core.Result.Result<bool>>; 