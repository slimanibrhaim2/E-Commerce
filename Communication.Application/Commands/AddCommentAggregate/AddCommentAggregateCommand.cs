using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.AddCommentAggregate
{
    public record AddCommentAggregateCommand(AddCommentAggregateDTO DTO, Guid UserId) : IRequest<Result<Guid>>;
} 