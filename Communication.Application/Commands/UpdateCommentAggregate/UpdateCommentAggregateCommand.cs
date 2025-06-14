using MediatR;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.UpdateCommentAggregate;

public record UpdateCommentAggregateCommand(Guid Id, AddCommentAggregateDTO DTO) : IRequest<Core.Result.Result<Guid>>; 