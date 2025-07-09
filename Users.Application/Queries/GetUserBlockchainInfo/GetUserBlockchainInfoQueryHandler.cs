using Core.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.DTOs.Blockchain;
using Shared.Contracts.Queries;
using Users.Domain.Repositories;

namespace Users.Application.Queries.GetUserBlockchainInfo;

public class GetUserBlockchainInfoQueryHandler : IRequestHandler<GetUserBlockchainInfoQuery, Result<UserBlockchainDTO>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserBlockchainInfoQueryHandler> _logger;

    public GetUserBlockchainInfoQueryHandler(
        IUserRepository userRepository,
        ILogger<GetUserBlockchainInfoQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<UserBlockchainDTO>> Handle(GetUserBlockchainInfoQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<UserBlockchainDTO>.Fail(
                    message: "User not found",
                    errorType: "UserNotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            var userBlockchainInfo = new UserBlockchainDTO
            {
                Username = user.FirstName + " " +user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            return Result<UserBlockchainDTO>.Ok(
                data: userBlockchainInfo,
                message: "User blockchain info retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving blockchain user info for user {UserId}", request.UserId);
            return Result<UserBlockchainDTO>.Fail(
                message: "Error retrieving user info",
                errorType: "GetUserInfoFailed",
                resultStatus: ResultStatus.Failed);
        }
    }
} 