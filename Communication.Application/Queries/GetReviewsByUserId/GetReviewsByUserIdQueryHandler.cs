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

namespace Communication.Application.Queries.GetReviewsByUserId;

public class GetReviewsByUserIdQueryHandler : IRequestHandler<GetReviewsByUserIdQuery, Result<PaginatedResult<ReviewDTO>>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetReviewsByUserIdQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<PaginatedResult<ReviewDTO>>> Handle(GetReviewsByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var reviews = await _reviewRepository.GetAllByUserIdAsync(request.UserId);
            var reviewsList = reviews.ToList();
            var totalCount = reviewsList.Count;
            
            // Get paginated reviews
            var paginatedReviews = reviewsList
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
                message: "تم جلب مراجعات المستخدم بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<PaginatedResult<ReviewDTO>>.Fail(
                message: $"فشل في جلب مراجعات المستخدم: {ex.Message}",
                errorType: "GetReviewsByUserIdFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 