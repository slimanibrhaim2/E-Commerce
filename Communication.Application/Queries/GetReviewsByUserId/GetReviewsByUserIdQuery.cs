using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Core.Pagination;
using System;

namespace Communication.Application.Queries.GetReviewsByUserId;

public record GetReviewsByUserIdQuery(Guid UserId, PaginationParameters Parameters) : IRequest<Result<PaginatedResult<ReviewDTO>>>; 