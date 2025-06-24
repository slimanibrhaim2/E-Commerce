using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Core.Pagination;
using System;

namespace Communication.Application.Queries.GetReviewsByBaseItemId;

public record GetReviewsByBaseItemIdQuery(Guid BaseItemId, PaginationParameters Parameters) : IRequest<Result<PaginatedResult<ReviewDTO>>>; 