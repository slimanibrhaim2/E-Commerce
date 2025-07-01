using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Core.Pagination;
using System;

namespace Communication.Application.Queries.GetReviewsByProviderId;

public record GetReviewsByProviderIdQuery(Guid ProviderId, PaginationParameters Parameters) : IRequest<Result<PaginatedResult<ReviewDTO>>>; 