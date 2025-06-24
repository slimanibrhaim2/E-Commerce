using MediatR;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.UpdateCommentAggregate;

public record UpdateCommentAggregateCommand(Guid Id, AddCommentAggregateDTO DTO, Guid UserId) : IRequest<Core.Result.Result<Guid>>; 