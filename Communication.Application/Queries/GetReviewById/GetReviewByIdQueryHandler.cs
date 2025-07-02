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
                return Result<ReviewDTO>.Fail(
                    message: "المراجعة غير موجودة",
                    errorType: "ReviewNotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            var reviewDto = new ReviewDTO
            {
                Id = review.Id,
                ExperienceDescription = review.ExperienceDescription,
                OverallSatisfaction = review.OverallSatisfaction,
                ItemQuality = review.ItemQuality,
                Communication = review.Communication,
                Timeliness = review.Timeliness,
                ValueForMoney = review.ValueForMoney,
                NetPromoterScore = review.NetPromoterScore,
                WillUseAgain = review.WillUseAgain,
                Rating = review.Rating,
                ReviewerId = review.ReviewerId,
                ProviderId = review.ProviderId,
                OrderId = review.OrderId,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };

            return Result<ReviewDTO>.Ok(
                data: reviewDto,
                message: "تم جلب المراجعة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<ReviewDTO>.Fail(
                message: $"فشل في جلب المراجعة: {ex.Message}",
                errorType: "GetReviewByIdFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 