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

namespace Users.Application.Commands.UpdateUser
{
    public class UpdateUserCommandHandler
        : IRequestHandler<UpdateUserCommand, Result>
    {
        private readonly IUserRepository _repo;
        private readonly IUnitOfWork _uow;

        public UpdateUserCommandHandler(IUserRepository repo, IUnitOfWork uow)
            => (_repo, _uow) = (repo, uow);

        public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dto = request.User;

                // 1. Load full user aggregate (with Addresses)
                var user = await _repo.GetByIdWithDetails(dto.Id)
                           ?? throw new KeyNotFoundException($"User with Id {dto.Id} not found.");

                // 2. Update scalar properties
                user.FirstName = dto.FirstName;
                user.MiddleName = dto.MiddleName;
                user.LastName = dto.LastName;
                user.PhoneNumber = dto.PhoneNumber;
                user.Email = dto.Email;
                user.ProfilePhoto = dto.ProfilePhoto;
                user.Description = dto.Description;

                // Update audit field
                user.UpdateUpdatedAt();

                // 3. Mark for update
                _repo.Update(user);

                // 4. Commit
                await _uow.SaveChangesAsync();

                return Result.Ok(
                    message: "تم تحديث بيانات المستخدم بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (KeyNotFoundException knf)
            {
                return Result.Fail(
                    message: "المستخدم غير موجود",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.ValidationError,
                    exception: knf);
            }
            catch (Exception ex)
            {
                return Result.Fail(
                    message: "حدث خطأ أثناء تحديث بيانات المستخدم",
                    errorType: "UpdateFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
