using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Communication.Application.Queries.GetReviewById;

public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, Result<ReviewDTO>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetReviewByIdQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<ReviewDTO>> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var review = await _reviewRepository.GetByIdAsync(request.ReviewId);
            if (review == null)
            {
                return Result<ReviewDTO>.Fail("المراجعة غير موجودة", "ReviewNotFound", ResultStatus.NotFound);
            }

            var reviewDto = new ReviewDTO
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
            };

            return Result<ReviewDTO>.Ok(reviewDto, "تم جلب المراجعة بنجاح", ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<ReviewDTO>.Fail($"فشل في جلب المراجعة: {ex.Message}", "GetReviewByIdFailed", ResultStatus.Failed, ex);
        }
    }
} 