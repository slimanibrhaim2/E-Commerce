using Core.Result;
using MediatR;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Users.Application.Command.AddFollowerByUserId;

public class AddFollowerByUserIdCommandHandler : IRequestHandler<AddFollowerByUserIdCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IFollowerRepository _followerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddFollowerByUserIdCommandHandler> _logger;

    public AddFollowerByUserIdCommandHandler(
        IUserRepository userRepository,
        IFollowerRepository followerRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddFollowerByUserIdCommandHandler> logger)
    {
        _userRepository = userRepository;
        _followerRepository = followerRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(AddFollowerByUserIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Attempting to add follower. FollowerId: {FollowerId}, FollowingId: {FollowingId}", 
                request.FollowerId, request.FollowingId);

            // Validation
            if (request.FollowingId == Guid.Empty)
            {
                _logger.LogWarning("Invalid FollowingId: {FollowingId}", request.FollowingId);
                return Result<Guid>.Fail(
                    message: "معرف المستخدم المتابَع غير صالح",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.FollowerId == Guid.Empty)
            {
                _logger.LogWarning("Invalid FollowerId: {FollowerId}", request.FollowerId);
                return Result<Guid>.Fail(
                    message: "معرف المستخدم المتابِع غير صالح",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.FollowerId == request.FollowingId)
            {
                _logger.LogWarning("User attempted to follow themselves. UserId: {UserId}", request.FollowerId);
                return Result<Guid>.Fail(
                    message: "لا يمكن للمستخدم متابعة نفسه",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            // Check if following user exists
            var followingUser = await _userRepository.GetByIdWithDetails(request.FollowingId);
            if (followingUser == null)
            {
                _logger.LogWarning("User not found with ID: {FollowingId}", request.FollowingId);
                return Result<Guid>.Fail(
                    message: "المستخدم المتابَع غير موجود",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.ValidationError);
            }

            // Check if follower user exists
            var followerUser = await _userRepository.GetByIdWithDetails(request.FollowerId);
            if (followerUser == null)
            {
                _logger.LogWarning("User not found with ID: {FollowerId}", request.FollowerId);
                return Result<Guid>.Fail(
                    message: "المستخدم المتابِع غير موجود",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.ValidationError);
            }

            // Initialize Followees collection if null
            followingUser.Followees ??= new List<Follower>();

            Follower follower = new Follower
            {
                Id = Guid.NewGuid(),
                FollowerId = request.FollowerId,
                FollowingId = request.FollowingId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _followerRepository.AddAsync(follower);
            followingUser.Followees.Add(follower);
            _userRepository.Update(followingUser);
            try
            {
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Successfully added follower. FollowerId: {FollowerId}, FollowingId: {FollowingId}", 
                    request.FollowerId, request.FollowingId);
                
                return Result<Guid>.Ok(
                    data: follower.Id,
                    message: "تم إضافة المتابع بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes while adding follower. FollowerId: {FollowerId}, FollowingId: {FollowingId}", 
                    request.FollowerId, request.FollowingId);
                return Result<Guid>.Fail(
                    message: "فشل في إضافة المتابع",
                    errorType: "AddFollowerFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while adding follower. FollowerId: {FollowerId}, FollowingId: {FollowingId}", 
                request.FollowerId, request.FollowingId);
            return Result<Guid>.Fail(
                message: "فشل في إضافة المتابع",
                errorType: "AddFollowerFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
}
