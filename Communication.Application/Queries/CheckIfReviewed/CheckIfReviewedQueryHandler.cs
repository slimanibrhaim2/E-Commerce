using MediatR;
using Core.Result;
using Communication.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Communication.Application.Queries.CheckIfReviewed;

public class CheckIfReviewedQueryHandler : IRequestHandler<CheckIfReviewedQuery, Result<bool>>
{
    private readonly IReviewRepository _reviewRepository;

    public CheckIfReviewedQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<bool>> Handle(CheckIfReviewedQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var hasReviewed = await _reviewRepository.HasUserReviewedOrderAsync(request.UserId, request.OrderId);
            
            return Result<bool>.Ok(
                data: hasReviewed,
                message: "تم فحص حالة المراجعة بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(
                message: $"فشل في فحص حالة المراجعة: {ex.Message}",
                errorType: "CheckIfReviewedFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 