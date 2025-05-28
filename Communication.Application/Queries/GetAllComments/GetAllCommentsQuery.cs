using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System.Collections.Generic;

namespace Communication.Application.Queries.GetAllComments;

public record GetAllCommentsQuery() : IRequest<Result<List<CommentDTO>>>; 