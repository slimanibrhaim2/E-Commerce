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
            var reviews = await _reviewRepository.GetByReviewerIdAsync(request.UserId, request.Parameters);

            // Map to DTOs
            var data = reviews.Data.Select(review => new ReviewDTO
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
            }).ToList();

            var result = PaginatedResult<ReviewDTO>.Create(
                data: data,
                pageNumber: request.Parameters.PageNumber,
                pageSize: request.Parameters.PageSize,
                totalCount: reviews.TotalCount);

            return Result<PaginatedResult<ReviewDTO>>.Ok(
                data: result,
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