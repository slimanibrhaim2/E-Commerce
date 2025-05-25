// Users.Application/Commands/UpdateUser/UpdateUserCommandHandler.cs
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Result;
using Core.Interfaces;
using Users.Application.Command.UpdateUser;
using Users.Application.DTOs;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Users.Application.Commands.UpdateUser
{
    public class UpdateUserCommandHandler
        : IRequestHandler<UpdateUserCommand, Result>
    {
        private readonly IUserRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        public UpdateUserCommandHandler(IUserRepository repo, IUnitOfWork uow, ILogger<UpdateUserCommandHandler> logger)
            => (_repo, _uow, _logger) = (repo, uow, logger);

        public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to update user with ID: {UserId}", request.User.Id);

                // Validation
                if (request.User.Id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid UserId");
                    return Result.Fail(
                        message: "معرف المستخدم غير صالح",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (string.IsNullOrWhiteSpace(request.User.FirstName))
                {
                    _logger.LogWarning("Invalid FirstName");
                    return Result.Fail(
                        message: "الاسم الأول مطلوب",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (string.IsNullOrWhiteSpace(request.User.LastName))
                {
                    _logger.LogWarning("Invalid LastName");
                    return Result.Fail(
                        message: "الاسم الأخير مطلوب",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (string.IsNullOrWhiteSpace(request.User.Email))
                {
                    _logger.LogWarning("Invalid Email");
                    return Result.Fail(
                        message: "البريد الإلكتروني مطلوب",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (string.IsNullOrWhiteSpace(request.User.PhoneNumber))
                {
                    _logger.LogWarning("Invalid PhoneNumber");
                    return Result.Fail(
                        message: "رقم الهاتف مطلوب",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Email format validation
                if (!IsValidEmail(request.User.Email))
                {
                    _logger.LogWarning("Invalid email format: {Email}", request.User.Email);
                    return Result.Fail(
                        message: "صيغة البريد الإلكتروني غير صحيحة",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Phone number format validation
                if (!IsValidPhoneNumber(request.User.PhoneNumber))
                {
                    _logger.LogWarning("Invalid phone number format: {PhoneNumber}", request.User.PhoneNumber);
                    return Result.Fail(
                        message: "صيغة رقم الهاتف غير صحيحة",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Load user
                var user = await _repo.GetByIdWithDetails(request.User.Id);
                if (user is null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", request.User.Id);
                    return Result.Fail(
                        message: "المستخدم غير موجود",
                        errorType: "NotFound",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Check email uniqueness if changed
                if (user.Email != request.User.Email)
                {
                    var existing = await _repo.GetByEmail(request.User.Email);
                    if (existing is not null)
                    {
                        _logger.LogWarning("Email already exists: {Email}", request.User.Email);
                        return Result.Fail(
                            message: $"البريد الإلكتروني '{request.User.Email}' مستخدم بالفعل",
                            errorType: "ValidationError",
                            resultStatus: ResultStatus.ValidationError);
                    }
                }

                // Update user
                user.FirstName = request.User.FirstName;
                user.MiddleName = request.User.MiddleName;
                user.LastName = request.User.LastName;
                user.PhoneNumber = request.User.PhoneNumber;
                user.Email = request.User.Email;
                user.ProfilePhoto = request.User.ProfilePhoto;
                user.Description = request.User.Description;

               

                _repo.Update(user);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("Successfully updated user with ID: {UserId}", user.Id);
                return Result.Ok(
                    message: "تم تحديث بيانات المستخدم بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", request.User.Id);
                return Result.Fail(
                    message: "فشل في تحديث بيانات المستخدم",
                    errorType: "UpdateFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Basic validation - can be enhanced based on your requirements
            return !string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.Length >= 10;
        }
    }
}
