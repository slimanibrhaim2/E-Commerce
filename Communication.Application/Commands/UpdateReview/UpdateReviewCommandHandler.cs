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
                var userId = request.UserId;
                var reviewId = request.ReviewId;
                
                // 1. Validate input
                if (string.IsNullOrWhiteSpace(dto.Title))
                    return Result<bool>.Fail("عنوان المراجعة مطلوب", "ValidationError", ResultStatus.ValidationError);
                if (string.IsNullOrWhiteSpace(dto.Content))
                    return Result<bool>.Fail("محتوى المراجعة مطلوب", "ValidationError", ResultStatus.ValidationError);

                // 2. Get existing review
                var existingReview = await _reviewRepo.GetByIdAsync(reviewId);
                if (existingReview == null)
                {
                    return Result<bool>.Fail("المراجعة غير موجودة", "ReviewNotFound", ResultStatus.NotFound);
                }

                // 3. Check ownership
                if (existingReview.UserId != userId)
                {
                    return Result<bool>.Fail("غير مسموح لك بتعديل هذه المراجعة", "Unauthorized", ResultStatus.ValidationError);
                }

                // 4. Update review
                existingReview.Title = dto.Title;
                existingReview.Content = dto.Content;
                existingReview.UpdatedAt = DateTime.UtcNow;

                _reviewRepo.Update(existingReview);
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