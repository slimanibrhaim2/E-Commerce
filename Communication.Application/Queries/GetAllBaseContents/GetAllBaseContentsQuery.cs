using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System.Collections.Generic;
using Core.Pagination;

namespace Communication.Application.Queries.GetAllBaseContents;

public record GetAllBaseContentsQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<BaseContentDTO>>>; 