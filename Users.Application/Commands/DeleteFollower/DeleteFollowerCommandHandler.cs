using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Result;
using Core.Interfaces;
using Users.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Users.Application.Commands.DeleteFollower
{
    public class DeleteFollowerCommandHandler : IRequestHandler<DeleteFollowerCommand, Result>
    {
        private readonly IFollowerRepository _followerRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<DeleteFollowerCommandHandler> _logger;

        public DeleteFollowerCommandHandler(IFollowerRepository followerRepo, IUnitOfWork uow, ILogger<DeleteFollowerCommandHandler> logger)
            => (_followerRepo, _uow, _logger) = (followerRepo, uow, logger);

        public async Task<Result> Handle(DeleteFollowerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to soft delete follower with ID: {FollowerId}", request.FollowerId);

                // Find the follower entity directly
                var follower = await _followerRepo.GetByIdAsync(request.FollowerId);
                if (follower is null)
                {
                    _logger.LogWarning("Follower not found with ID: {FollowerId}", request.FollowerId);
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
                _followerRepo.Update(follower);
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