using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System.Collections.Generic;

namespace Communication.Application.Queries.GetAllCommentsByBaseItemId;

public record GetAllCommentsByBaseItemIdQuery(Guid BaseItemId) : IRequest<Result<List<CommentDTO>>>;