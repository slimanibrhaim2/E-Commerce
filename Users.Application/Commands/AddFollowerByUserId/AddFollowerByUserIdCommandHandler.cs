using Core.Result;
using MediatR;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Core.Interfaces;

namespace Users.Application.Command.AddFollowerByUserId
{
    public class AddFollowerByUserIdCommandHandler : IRequestHandler<AddFollowerByUserIdCommand, Result<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddFollowerByUserIdCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(AddFollowerByUserIdCommand request, CancellationToken cancellationToken)
        {
            var followingUser = await _userRepository.GetByIdWithDetails(request.FollowingId);
            if (followingUser == null)
            {
                return Result<Guid>.Fail(
                    message: "User not found",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (request.FollowerId == request.FollowingId)
            {
                return Result<Guid>.Fail(
                    message: "User cannot follow themselves",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            if (followingUser.Followees.Exists(f => f.FollowerId == request.FollowerId))
            {
                return Result<Guid>.Fail(
                    message: "User is already following this user",
                    errorType: "ValidationError",
                    resultStatus: ResultStatus.ValidationError);
            }

            var follower = new Follower
            {
                Id = Guid.NewGuid(),
                FollowerId = request.FollowerId,
                FollowingId = request.FollowingId,
                FollowedAt = DateTime.UtcNow
            };

            followingUser.Followees.Add(follower);
            _userRepository.Update(followingUser);
            await _unitOfWork.SaveChangesAsync();

            return Result<Guid>.Ok(
                data: follower.Id,
                message: "Follower added successfully",
                resultStatus: ResultStatus.Success);
        }
    }
} 