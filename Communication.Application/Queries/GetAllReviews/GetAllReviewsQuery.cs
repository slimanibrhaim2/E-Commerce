using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using System.Collections.Generic;
using Core.Pagination;

namespace Communication.Application.Queries.GetAllReviews;

public record GetAllReviewsQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<ReviewDTO>>>; 