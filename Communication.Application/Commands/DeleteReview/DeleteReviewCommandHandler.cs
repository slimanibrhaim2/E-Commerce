using MediatR;
using Core.Result;
using Communication.Domain.Repositories;
using Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Communication.Application.Commands.DeleteReview
{
    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReviewRepository _reviewRepo;

        public DeleteReviewCommandHandler(
            IUnitOfWork unitOfWork,
            IReviewRepository reviewRepo)
        {
            _unitOfWork = unitOfWork;
            _reviewRepo = reviewRepo;
        }

        public async Task<Result<bool>> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransaction();
            try
            {
                var userId = request.UserId;
                var reviewId = request.ReviewId;
                
                // 1. Get existing review
                var existingReview = await _reviewRepo.GetByIdAsync(reviewId);
                if (existingReview == null)
                {
                    return Result<bool>.Fail("المراجعة غير موجودة", "ReviewNotFound", ResultStatus.NotFound);
                }

                // 2. Check ownership
                if (existingReview.UserId != userId)
                {
                    return Result<bool>.Fail("غير مسموح لك بحذف هذه المراجعة", "Unauthorized", ResultStatus.ValidationError);
                }

                // 3. Soft delete review
                _reviewRepo.Remove(existingReview);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                return Result<bool>.Ok(true, "تم حذف المراجعة بنجاح", ResultStatus.Success);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransaction();
                return Result<bool>.Fail($"فشل في حذف المراجعة: {ex.Message}", "DeleteReviewFailed", ResultStatus.Failed, ex);
            }
        }
    }
} 