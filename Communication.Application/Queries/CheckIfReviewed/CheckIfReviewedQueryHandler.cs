using MediatR;
using Core.Result;
using Communication.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.Contracts.Queries;

namespace Communication.Application.Queries.CheckIfReviewed;

public class CheckIfReviewedQueryHandler : IRequestHandler<CheckIfReviewedQuery, Result<bool>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMediator _mediator;

    public CheckIfReviewedQueryHandler(IReviewRepository reviewRepository, IMediator mediator)
    {
        _reviewRepository = reviewRepository;
        _mediator = mediator;
    }

    public async Task<Result<bool>> Handle(CheckIfReviewedQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Resolve BaseItemId from ItemId
            var productQuery = new GetBaseItemIdByProductIdQuery(request.ItemId);
            var productResult = await _mediator.Send(productQuery, cancellationToken);
            
            Guid baseItemId;
            if (productResult.Success)
            {
                baseItemId = productResult.Data;
            }
            else
            {
                var serviceQuery = new GetBaseItemIdByServiceIdQuery(request.ItemId);
                var serviceResult = await _mediator.Send(serviceQuery, cancellationToken);
                
                if (serviceResult.Success)
                {
                    baseItemId = serviceResult.Data;
                }
                else
                {
                    return Result<bool>.Fail("العنصر غير موجود", "ItemNotFound", ResultStatus.NotFound);
                }
            }

            // 2. Check if user has reviewed this item
            var hasReviewed = await _reviewRepository.HasUserReviewedItemAsync(request.UserId, baseItemId);
            
            return Result<bool>.Ok(hasReviewed, "تم فحص حالة المراجعة بنجاح", ResultStatus.Success);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail($"فشل في فحص حالة المراجعة: {ex.Message}", "CheckIfReviewedFailed", ResultStatus.Failed, ex);
        }
    }
} 