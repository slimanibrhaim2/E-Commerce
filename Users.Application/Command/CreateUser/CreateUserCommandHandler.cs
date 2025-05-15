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

namespace Users.Application.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
    {
        private readonly IUserRepository _repo;
        private readonly IUnitOfWork _uow;

        public CreateUserCommandHandler(IUserRepository repo, IUnitOfWork uow)
            => (_repo, _uow) = (repo, uow);

        public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dto = request.User;

                // 0. Duplication check
                var existing = await _repo.GetByEmail(dto.Email);
                if (existing is not null)
                    return Result<Guid>.Fail(
                        message: $"البريد الإلكتروني '{dto.Email}' مستخدم بالفعل",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);

                // 1. Map DTO → Entity (no addresses, no followers)
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = dto.FirstName,
                    MiddleName = dto.MiddleName,
                    LastName = dto.LastName,
                    PhoneNumber = dto.PhoneNumber,
                    Email = dto.Email,
                    ProfilePhoto = dto.ProfilePhoto,
                    Description = dto.Description,
                };

                // Initialize audit fields
                user.InitCreatedAt();
                user.UpdateUpdatedAt();

                // 2. Persist
                await _repo.AddAsync(user);
                await _uow.SaveChangesAsync();

                return Result<Guid>.Ok(
                    data: user.Id,
                    message: "تم الإنشاء بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Fail(
                    message: "حدث خطأ أثناء إنشاء المستخدم",
                    errorType: "CreateFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
