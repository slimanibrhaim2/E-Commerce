using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Result;
using Users.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Users.Application.Commands.AddFollowerByUserId
{
    public class DeleteFollowerCommandHandler : IRequestHandler<DeleteFollowerCommand, Result>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<DeleteFollowerCommandHandler> _logger;

        public DeleteFollowerCommandHandler(IUserRepository userRepo, IUnitOfWork uow, ILogger<DeleteFollowerCommandHandler> logger)
            => (_userRepo, _uow, _logger) = (userRepo, uow, logger);

        public async Task<Result> Handle(DeleteFollowerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to soft delete follower with ID: {FollowerId}", request.FollowerId);

                // Find the user containing this follower
                var user = await _userRepo.GetByFollowerId(request.FollowerId);
                if (user is null)
                {
                    _logger.LogWarning("Follower not found with ID: {FollowerId}", request.FollowerId);
                    return Result.Fail(
                        message: "المتابع غير موجود",
                        errorType: "NotFound",
                        resultStatus: ResultStatus.ValidationError);
                }

                var follower = user.Followees?.Find(f => f.Id == request.FollowerId);
                if (follower is null)
                {
                    _logger.LogWarning("Follower not found in user aggregate. FollowerId: {FollowerId}", request.FollowerId);
                    return Result.Fail(
                        message: "المتابع غير موجود",
                        errorType: "NotFound",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (follower.DeletedAt != null)
                {
                    _logger.LogWarning("Follower already deleted. FollowerId: {FollowerId}", request.FollowerId);
                    return Result.Fail(
                        message: "المتابع محذوف بالفعل",
                        errorType: "AlreadyDeleted",
                        resultStatus: ResultStatus.ValidationError);
                }

                follower.DeletedAt = DateTime.UtcNow;
                _userRepo.Update(user);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("Successfully soft deleted follower with ID: {FollowerId}", request.FollowerId);
                return Result.Ok(
                    message: "تم حذف المتابع بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft deleting follower with ID: {FollowerId}", request.FollowerId);
                return Result.Fail(
                    message: "فشل في حذف المتابع",
                    errorType: "DeleteFollowerFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 