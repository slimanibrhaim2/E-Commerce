using MediatR;
using Core.Result;
using Communication.Application.DTOs;

namespace Communication.Application.Queries.GetCommentById;

public record GetCommentByIdQuery(Guid Id) : IRequest<Result<CommentDTO>>; 