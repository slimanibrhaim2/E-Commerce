using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.Contracts.Queries;

namespace Communication.Application.Commands.CreateReview
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReviewRepository _reviewRepo;
        private readonly IMediator _mediator;

        public CreateReviewCommandHandler(
            IUnitOfWork unitOfWork,
            IReviewRepository reviewRepo,
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _reviewRepo = reviewRepo;
            _mediator = mediator;
        }

        public async Task<Result<Guid>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var dto = request.DTO;
                var userId = request.UserId;
                
                // 1. Validate input
                if (string.IsNullOrWhiteSpace(dto.Title))
                    return Result<Guid>.Fail("عنوان المراجعة مطلوب", "ValidationError", ResultStatus.ValidationError);
                if (string.IsNullOrWhiteSpace(dto.Content))
                    return Result<Guid>.Fail("محتوى المراجعة مطلوب", "ValidationError", ResultStatus.ValidationError);
                if (dto.ItemId == Guid.Empty)
                    return Result<Guid>.Fail("معرف العنصر مطلوب", "ValidationError", ResultStatus.ValidationError);
                if (dto.OrderId == Guid.Empty)
                    return Result<Guid>.Fail("معرف الطلب مطلوب", "ValidationError", ResultStatus.ValidationError);

                // 2. Resolve BaseItemId from ItemId
                var productQuery = new GetBaseItemIdByProductIdQuery(dto.ItemId);
                var productResult = await _mediator.Send(productQuery, cancellationToken);
                
                Guid baseItemId;
                if (productResult.Success)
                {
                    baseItemId = productResult.Data;
                }
                else
                {
                    var serviceQuery = new GetBaseItemIdByServiceIdQuery(dto.ItemId);
                    var serviceResult = await _mediator.Send(serviceQuery, cancellationToken);
                    
                    if (serviceResult.Success)
                    {
                        baseItemId = serviceResult.Data;
                    }
                    else
                    {
                        return Result<Guid>.Fail("العنصر غير موجود", "ItemNotFound", ResultStatus.NotFound);
                    }
                }

                // 3. Check if user already reviewed this item
                var hasReviewed = await _reviewRepo.HasUserReviewedItemAsync(userId, baseItemId);
                if (hasReviewed)
                {
                    return Result<Guid>.Fail("لقد قمت بمراجعة هذا العنصر من قبل", "DuplicateReview", ResultStatus.ValidationError);
                }

                // 4. Verify purchase
                var hasPurchased = await _reviewRepo.HasUserPurchasedItemAsync(userId, baseItemId, dto.OrderId);
                if (!hasPurchased)
                {
                    return Result<Guid>.Fail("يجب شراء العنصر أولاً لتتمكن من مراجعته", "PurchaseRequired", ResultStatus.ValidationError);
                }

                // 5. Create Review
                var review = new Review
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    BaseItemId = baseItemId,
                    OrderId = dto.OrderId,
                    Title = dto.Title,
                    Content = dto.Content,
                    IsVerifiedPurchase = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _reviewRepo.AddAsync(review);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return Result<Guid>.Ok(review.Id, "تم إضافة المراجعة بنجاح", ResultStatus.Success);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return Result<Guid>.Fail($"فشل في إضافة المراجعة: {ex.Message}", "CreateReviewFailed", ResultStatus.Failed, ex);
            }
        }
    }
} 