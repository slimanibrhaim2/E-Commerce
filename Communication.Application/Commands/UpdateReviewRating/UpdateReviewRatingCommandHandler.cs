using MediatR;
using Core.Result;
using Communication.Domain.Repositories;
using Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.Contracts.Commands;

namespace Communication.Application.Commands.UpdateReviewRating
{
    public class UpdateReviewRatingCommandHandler : IRequestHandler<UpdateReviewRatingCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReviewRepository _reviewRepo;

        public UpdateReviewRatingCommandHandler(
            IUnitOfWork unitOfWork,
            IReviewRepository reviewRepo)
        {
            _unitOfWork = unitOfWork;
            _reviewRepo = reviewRepo;
        }

        public async Task<Result<bool>> Handle(UpdateReviewRatingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Validate rating before starting transaction
                if (request.Rating < 1 || request.Rating > 5)
                {
                    return Result<bool>.Fail(
                        message: "التقييم يجب أن يكون بين 1 و 5",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                await _unitOfWork.BeginTransaction();

                try
                {
                    var review = await _reviewRepo.GetByIdAsync(request.ReviewId);
                    if (review == null)
                    {
                        await _unitOfWork.RollbackTransaction();
                        return Result<bool>.Fail(
                            message: "المراجعة غير موجودة",
                            errorType: "ReviewNotFound",
                            resultStatus: ResultStatus.NotFound);
                    }

                    review.Rating = request.Rating;
                    review.UpdatedAt = DateTime.UtcNow;

                    await _reviewRepo.UpdateAsync(review);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransaction();

                    return Result<bool>.Ok(
                        data: true,
                        message: "تم تحديث تقييم المراجعة بنجاح",
                        resultStatus: ResultStatus.Success);
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackTransaction();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(
                    message: $"فشل في تحديث تقييم المراجعة: {ex.Message}",
                    errorType: "UpdateReviewRatingFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 