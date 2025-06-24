using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Pagination;
using System;

namespace Communication.Application.Queries.GetAllReviews;

public class GetAllReviewsQueryHandler : IRequestHandler<GetAllReviewsQuery, Result<PaginatedResult<ReviewDTO>>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetAllReviewsQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<PaginatedResult<ReviewDTO>>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var reviews = await _reviewRepository.GetAllAsync();
            var totalCount = reviews.Count();
            
            // Get paginated reviews
            var paginatedReviews = reviews
                .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                .Take(request.Parameters.PageSize)
                .ToList();

            // Map to DTOs
            var data = paginatedReviews.Select(review => new ReviewDTO
            {
                Id = review.Id,
                UserId = review.UserId,
                BaseItemId = review.BaseItemId,
                OrderId = review.OrderId,
                Title = review.Title,
                Content = review.Content,
                IsVerifiedPurchase = review.IsVerifiedPurchase,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                DeletedAt = review.DeletedAt
            }).ToList();

            var paginated = Core.Pagination.PaginatedResult<ReviewDTO>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<ReviewDTO>>.Ok(
                paginated,
                message: "تم جلب المراجعات بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<PaginatedResult<ReviewDTO>>.Fail(
                message: $"فشل في جلب المراجعات: {ex.Message}",
                errorType: "GetAllReviewsFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 