// Users.Application/Queries/GetAllUsers/GetAllUsersQueryHandler.cs
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Result;
using Users.Application.DTOs;
using Users.Domain.Repositories;
using Core.Pagination;
using Microsoft.Extensions.Logging;

namespace Users.Application.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler
        : IRequestHandler<GetAllUsersQuery, Result<PaginatedResult<UserDTO>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRatingRepository _userRatingRepository;
        private readonly ILogger<GetAllUsersQueryHandler> _logger;

        public GetAllUsersQueryHandler(
            IUserRepository userRepository,
            IUserRatingRepository userRatingRepository,
            ILogger<GetAllUsersQueryHandler> logger)
        {
            _userRepository = userRepository;
            _userRatingRepository = userRatingRepository;
            _logger = logger;
        }

        public async Task<Result<PaginatedResult<UserDTO>>> Handle(
            GetAllUsersQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var usersList = users.ToList();
                var totalCount = usersList.Count;

                // Get all user ratings
                var userIds = usersList.Select(u => u.Id).ToList();
                var userRatings = (await Task.WhenAll(userIds.Select(id => 
                    _userRatingRepository.GetByUserIdAsync(id))))
                    .ToDictionary(r => r?.UserId ?? Guid.Empty, r => r);

                var userDtos = usersList.Select(user => new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    ProfilePhoto = user.ProfilePhoto,
                    Description = user.Description,
                    Rating = userRatings.TryGetValue(user.Id, out var rating) ? rating?.Rating ?? 3 : 3,
                    NumOfReviews = userRatings.TryGetValue(user.Id, out var reviews) ? reviews?.NumOfReviews ?? 0 : 0
                }).ToList();

                // Apply pagination
                var pageNumber = request.Parameters.PageNumber;
                var pageSize = request.Parameters.PageSize;
                var pagedUserDtos = userDtos
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var paginatedUsers = PaginatedResult<UserDTO>.Create(
                    data: pagedUserDtos,
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    totalCount: totalCount);

                return Result<PaginatedResult<UserDTO>>.Ok(
                    data: paginatedUsers,
                    message: "تم جلب المستخدمين بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return Result<PaginatedResult<UserDTO>>.Fail(
                    message: "حدث خطأ أثناء جلب المستخدمين",
                    errorType: "GetAllUsersFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
