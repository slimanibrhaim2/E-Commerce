// Users.Application/Commands/CreateUser/CreateUserCommandHandler.cs
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.DTOs;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Core.Interfaces;
using Core.Result;
using Microsoft.Extensions.Logging;

namespace Users.Application.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
    {
        private readonly IUserRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(IUserRepository repo, IUnitOfWork uow, ILogger<CreateUserCommandHandler> logger)
            => (_repo, _uow, _logger) = (repo, uow, logger);

        public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to create user with email: {Email}", request.userDTO.Email);

                // Validation
                if (string.IsNullOrWhiteSpace(request.userDTO.FirstName))
                {
                    _logger.LogWarning("Invalid FirstName");
                    return Result<Guid>.Fail(
                        message: "يرجى إدخال الاسم الأول",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (string.IsNullOrWhiteSpace(request.userDTO.LastName))
                {
                    _logger.LogWarning("Invalid LastName");
                    return Result<Guid>.Fail(
                        message: "يرجى إدخال الاسم الأخير",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (string.IsNullOrWhiteSpace(request.userDTO.PhoneNumber))
                {
                    _logger.LogWarning("Invalid PhoneNumber");
                    return Result<Guid>.Fail(
                        message: "يرجى إدخال رقم الهاتف",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (!IsValidPhoneNumber(request.userDTO.PhoneNumber))
                {
                    _logger.LogWarning("Invalid PhoneNumber format");
                    return Result<Guid>.Fail(
                        message: "يرجى إدخال رقم هاتف صحيح (يبدأ بـ 09 ويتكون من 10 أرقام)",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (string.IsNullOrWhiteSpace(request.userDTO.Email))
                {
                    _logger.LogWarning("Invalid Email");
                    return Result<Guid>.Fail(
                        message: "يرجى إدخال البريد الإلكتروني",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (!IsValidEmail(request.userDTO.Email))
                {
                    _logger.LogWarning("Invalid Email format");
                    return Result<Guid>.Fail(
                        message: "يرجى إدخال بريد إلكتروني صحيح",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Duplication check
                var existing = await _repo.GetByEmail(request.userDTO.Email);
                if (existing is not null)
                {
                    _logger.LogWarning("Email already exists: {Email}", request.userDTO.Email);
                    return Result<Guid>.Fail(
                        message: "البريد الإلكتروني مسجل مسبقاً في النظام",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Create user
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = request.userDTO.FirstName,
                    MiddleName = request.userDTO.MiddleName,
                    LastName = request.userDTO.LastName,
                    PhoneNumber = request.userDTO.PhoneNumber,
                    Email = request.userDTO.Email,
                    ProfilePhoto = request.userDTO.ProfilePhoto,
                    Description = request.userDTO.Description,
                };

                await _repo.AddAsync(user);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("Successfully created user with ID: {UserId}", user.Id);
                return Result<Guid>.Ok(
                    data: user.Id,
                    message: "تم إنشاء الحساب بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return Result<Guid>.Fail(
                    message: "حدث خطأ أثناء إنشاء الحساب",
                    errorType: "CreateUserFailed",
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
            
            return !string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.Length == 10 && phoneNumber.StartsWith("09");
        }
    }
}
