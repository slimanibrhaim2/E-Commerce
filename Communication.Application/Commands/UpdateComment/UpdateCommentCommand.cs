using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.UpdateComment;

public record UpdateCommentCommand(Guid Id, CreateCommentDTO Comment) : IRequest<Result<bool>>; 