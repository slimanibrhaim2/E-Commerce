using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Core.Pagination;
using System;

namespace Communication.Application.Queries.GetReviewsByOrderId;

public record GetReviewsByOrderIdQuery(Guid OrderId, PaginationParameters Parameters) : IRequest<Result<PaginatedResult<ReviewDTO>>>; 