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
                _logger.LogInformation("Attempting to soft delete follower relationship. FollowerId: {FollowerId}, FollowingId: {FollowingId}", 
                    request.FollowerId, request.FollowingId);

                // Find the follower relationship using both IDs
                var follower = await _followerRepo.GetByFollowerAndFollowingId(request.FollowerId, request.FollowingId);
                if (follower is null)
                {
                    _logger.LogWarning("Follower relationship not found. FollowerId: {FollowerId}, FollowingId: {FollowingId}", 
                        request.FollowerId, request.FollowingId);
                    return Result.Fail(
                        message: "علاقة المتابعة غير موجودة",
                        errorType: "NotFound",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (follower.DeletedAt != null)
                {
                    _logger.LogWarning("Follower relationship already deleted. FollowerId: {FollowerId}, FollowingId: {FollowingId}", 
                        request.FollowerId, request.FollowingId);
                    return Result.Fail(
                        message: "تم حذف علاقة المتابعة مسبقاً",
                        errorType: "AlreadyDeleted",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Use the new soft delete Remove method
                _followerRepo.Remove(follower);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("Successfully soft deleted follower relationship. FollowerId: {FollowerId}, FollowingId: {FollowingId}", 
                    request.FollowerId, request.FollowingId);
                return Result.Ok(
                    message: "تم حذف المتابع بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft deleting follower relationship. FollowerId: {FollowerId}, FollowingId: {FollowingId}", 
                    request.FollowerId, request.FollowingId);
                return Result.Fail(
                    message: "فشل في حذف المتابع",
                    errorType: "DeleteFollowerFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}