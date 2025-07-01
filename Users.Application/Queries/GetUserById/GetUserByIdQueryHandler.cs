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
        private readonly ILogger<GetUserByIdQueryHandler> _logger;

        public GetUserByIdQueryHandler(IUserRepository userRepository, ILogger<GetUserByIdQueryHandler> logger)
        {
            _userRepository = userRepository;
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

                var userDto = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName
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
                    message: $"فشل في جلب بيانات المستخدم: {ex.Message}",
                    errorType: "GetUserByIdFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
