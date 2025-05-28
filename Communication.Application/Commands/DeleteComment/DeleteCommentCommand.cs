using MediatR;
using Core.Result;

namespace Communication.Application.Commands.DeleteComment;

public record DeleteCommentCommand(Guid Id) : IRequest<Result<bool>>; 