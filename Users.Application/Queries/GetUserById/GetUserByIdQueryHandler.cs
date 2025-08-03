// Users.Application/Queries/GetUserById/GetUserByIdQueryHandler.cs
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.DTOs;
using Users.Domain.Repositories;
using Core.Result;
using Microsoft.Extensions.Logging;

namespace Users.Application.Queries.GetUserById
{
    public class GetUserByIdQueryHandler
        : IRequestHandler<GetUserByIdQuery, Result<UserDTO>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRatingRepository _userRatingRepository;
        private readonly IFollowerRepository _followerRepository;
        private readonly ILogger<GetUserByIdQueryHandler> _logger;

        public GetUserByIdQueryHandler(
            IUserRepository userRepository, 
            IUserRatingRepository userRatingRepository,
            IFollowerRepository followerRepository,
            ILogger<GetUserByIdQueryHandler> logger)
        {
            _userRepository = userRepository;
            _userRatingRepository = userRatingRepository;
            _followerRepository = followerRepository;
            _logger = logger;
        }

        public async Task<Result<UserDTO>> Handle(
            GetUserByIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to get user with ID: {UserId}", request.UserId);

                var user = await _userRepository.GetByIdWithDetails(request.UserId);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", request.UserId);
                    return Result<UserDTO>.Fail(
                        message: "المستخدم غير موجود",
                        errorType: "UserNotFound",
                        resultStatus: ResultStatus.NotFound);
                }

                // Get user rating
                var userRating = await _userRatingRepository.GetByUserIdAsync(request.UserId);

                // Check if the requestor follows this user
                bool isFollowed = false;
                if (request.RequestorUserId.HasValue && request.RequestorUserId.Value != Guid.Empty && request.RequestorUserId.Value != request.UserId)
                {
                    var followRelation = await _followerRepository.GetByFollowerAndFollowingId(request.RequestorUserId.Value, request.UserId);
                    isFollowed = followRelation != null;
                }

                var userDto = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    UserType = user.UserType,
                    ProfilePhoto = user.ProfilePhoto,
                    Description = user.Description,
                    Rating = userRating?.Rating ?? 3, // Default to 3 if no rating exists
                    NumOfReviews = userRating?.NumOfReviews ?? 0,
                    IsFollowed = isFollowed
                };

                return Result<UserDTO>.Ok(
                    data: userDto,
                    message: "تم جلب بيانات المستخدم بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user with ID: {UserId}", request.UserId);
                return Result<UserDTO>.Fail(
                    message: "حدث خطأ أثناء جلب بيانات المستخدم",
                    errorType: "GetUserByIdFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
