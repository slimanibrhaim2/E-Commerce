// Users.Application/Commands/DeleteUser/DeleteUserCommandHandler.cs
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Users.Domain.Repositories;
using Core.Result;
using Microsoft.Extensions.Logging;

namespace Users.Application.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly IUserRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<DeleteUserCommandHandler> _logger;

        public DeleteUserCommandHandler(IUserRepository repo, IUnitOfWork uow, ILogger<DeleteUserCommandHandler> logger)
            => (_repo, _uow, _logger) = (repo, uow, logger);

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to delete user with ID: {UserId}", request.Id);

                // Validation
                if (request.Id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid UserId");
                    return Result.Fail(
                        message: "معرف المستخدم غير صالح",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Load user with details
                var user = await _repo.GetByIdWithDetails(request.Id);
                if (user is null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", request.Id);
                    return Result.Fail(
                        message: "لم يتم العثور على المستخدم",
                        errorType: "NotFound",
                        resultStatus: ResultStatus.NotFound);
                }

                // Remove all followers
                if (user.Followees?.Any() == true)
                {
                    _logger.LogInformation("Removing {Count} followers for user {UserId}", user.Followees.Count, request.Id);
                    user.Followees.Clear();
                }

                // Remove all addresses
                if (user.Addresses?.Any() == true)
                {
                    _logger.LogInformation("Removing {Count} addresses for user {UserId}", user.Addresses.Count, request.Id);
                    user.Addresses.Clear();
                }

                _repo.Remove(user);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted user with ID: {UserId}", user.Id);
                return Result.Ok(
                    message: "تم حذف الحساب بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", request.Id);
                return Result.Fail(
                    message: "حدث خطأ أثناء حذف الحساب",
                    errorType: "DeleteFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
