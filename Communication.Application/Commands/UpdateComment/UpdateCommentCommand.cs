using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.UpdateComment;

public record UpdateCommentCommand(Guid Id, AddCommentAggregateDTO Comment, Guid UserId) : IRequest<Result<bool>>; 