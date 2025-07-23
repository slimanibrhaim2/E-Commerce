using MediatR;
using Core.Result;
using Users.Domain.Repositories;
using Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.Contracts.Commands;
using Users.Domain.Entities;

namespace Users.Application.Commands.UpdateUserRating
{
    public class UpdateUserRatingCommandHandler : IRequestHandler<UpdateUserRatingCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRatingRepository _userRatingRepo;

        public UpdateUserRatingCommandHandler(
            IUnitOfWork unitOfWork,
            IUserRatingRepository userRatingRepo)
        {
            _unitOfWork = unitOfWork;
            _userRatingRepo = userRatingRepo;
        }

        public async Task<Result<bool>> Handle(UpdateUserRatingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Validate rating
                if (request.Rating < 1 || request.Rating > 5)
                {
                    return Result<bool>.Fail(
                        message: "التقييم يجب أن يكون بين 1 و 5",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                // 2. Get or create user rating
                var userRating = await _userRatingRepo.GetByUserIdAsync(request.UserId);
                
                if (userRating == null)
                {
                    userRating = new UserRating
                    {
                        Id = Guid.NewGuid(),
                        UserId = request.UserId,
                        Rating = 3, // Default rating
                        NumOfReviews = 0, // Initial number of reviews
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _userRatingRepo.AddAsync(userRating);
                }

                // 3. Calculate new rating
                var newRating = (request.Rating + userRating.NumOfReviews * userRating.Rating) / (userRating.NumOfReviews + 1);
                
                // 4. Update user rating
                userRating.Rating = newRating;
                userRating.NumOfReviews++;
                userRating.UpdatedAt = DateTime.UtcNow;

                _userRatingRepo.Update(userRating);

                return Result<bool>.Ok(
                    data: true,
                    message: "تم تحديث تقييم المستخدم بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(
                    message: $"فشل في تحديث تقييم المستخدم: {ex.Message}",
                    errorType: "UpdateUserRatingFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 