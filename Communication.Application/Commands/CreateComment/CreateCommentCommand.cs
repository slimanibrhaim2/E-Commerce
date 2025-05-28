using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Commands.CreateComment;

public record CreateCommentCommand(CreateCommentDTO Comment) : IRequest<Result<Guid>>; 