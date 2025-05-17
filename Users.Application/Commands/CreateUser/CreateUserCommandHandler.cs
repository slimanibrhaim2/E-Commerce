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
                // 0. Duplication check
                var existing = await _repo.GetByEmail(request.userDTO.Email);
                if (existing is not null)
                    return Result<Guid>.Fail(
                        message: $"البريد الإلكتروني '{request.userDTO.Email}' مستخدم بالفعل",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);

                // 1. Map DTO → Entity (no addresses, no followers)
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
