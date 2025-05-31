using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System.Collections.Generic;
using Core.Pagination;

namespace Communication.Application.Queries.GetAllComments;

public record GetAllCommentsQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<CommentDTO>>>; 