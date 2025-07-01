using MediatR;
using Core.Result;
using Communication.Application.DTOs;
using Communication.Domain.Repositories;
using Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Communication.Application.Commands.UpdateReview
{
    public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReviewRepository _reviewRepo;

        public UpdateReviewCommandHandler(
            IUnitOfWork unitOfWork,
            IReviewRepository reviewRepo)
        {
            _unitOfWork = unitOfWork;
            _reviewRepo = reviewRepo;
        }

        public async Task<Result<bool>> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var dto = request.DTO;
                var reviewerId = request.UserId;
                var reviewId = request.ReviewId;

                // 1. Validate input
                if (string.IsNullOrWhiteSpace(dto.ExperienceDescription))
                    return Result<bool>.Fail("وصف التجربة مطلوب", "ValidationError", ResultStatus.ValidationError);
                
                if (dto.OverallSatisfaction < 1 || dto.OverallSatisfaction > 5)
                    return Result<bool>.Fail("التقييم العام يجب أن يكون بين 1 و 5", "ValidationError", ResultStatus.ValidationError);
                
                if (dto.ItemQuality < 1 || dto.ItemQuality > 5)
                    return Result<bool>.Fail("تقييم جودة المنتج يجب أن يكون بين 1 و 5", "ValidationError", ResultStatus.ValidationError);
                
                if (dto.Communication < 1 || dto.Communication > 5)
                    return Result<bool>.Fail("تقييم التواصل يجب أن يكون بين 1 و 5", "ValidationError", ResultStatus.ValidationError);
                
                if (dto.Timeliness < 1 || dto.Timeliness > 5)
                    return Result<bool>.Fail("تقييم الوقت يجب أن يكون بين 1 و 5", "ValidationError", ResultStatus.ValidationError);
                
                if (dto.NetPromoterScore < 0 || dto.NetPromoterScore > 10)
                    return Result<bool>.Fail("درجة التوصية يجب أن تكون بين 0 و 10", "ValidationError", ResultStatus.ValidationError);

                // 2. Get existing review
                var existingReview = await _reviewRepo.GetByIdAsync(reviewId);
                if (existingReview == null)
                {
                    return Result<bool>.Fail("المراجعة غير موجودة", "ReviewNotFound", ResultStatus.NotFound);
                }

                // 3. Verify ownership
                if (existingReview.ReviewerId != reviewerId)
                {
                    return Result<bool>.Fail("لا يمكنك تعديل مراجعة شخص آخر", "UnauthorizedAccess", ResultStatus.Unauthorized);
                }

                // 4. Update review
                existingReview.ExperienceDescription = dto.ExperienceDescription;
                existingReview.OverallSatisfaction = dto.OverallSatisfaction;
                existingReview.ItemQuality = dto.ItemQuality;
                existingReview.Communication = dto.Communication;
                existingReview.Timeliness = dto.Timeliness;
                existingReview.ValueForMoney = dto.ValueForMoney;
                existingReview.NetPromoterScore = dto.NetPromoterScore;
                existingReview.WillUseAgain = dto.WillUseAgain;
                existingReview.UpdatedAt = DateTime.UtcNow;

                await _reviewRepo.UpdateAsync(existingReview);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return Result<bool>.Ok(true, "تم تحديث المراجعة بنجاح", ResultStatus.Success);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return Result<bool>.Fail($"فشل في تحديث المراجعة: {ex.Message}", "UpdateReviewFailed", ResultStatus.Failed, ex);
            }
        }
    }
} 