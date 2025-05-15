// Users.Application/Commands/DeleteUser/DeleteUserCommandHandler.cs
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Users.Domain.Repositories;
using Core.Result;

namespace Users.Application.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly IUserRepository _repo;
        private readonly IUnitOfWork _uow;

        public DeleteUserCommandHandler(IUserRepository repo, IUnitOfWork uow)
            => (_repo, _uow) = (repo, uow);

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Load with details so EF can navigate children
                var user = await _repo.GetByIdWithDetails(request.Id)
                           ?? throw new KeyNotFoundException($"User with Id {request.Id} not found.");

                _repo.Remove(user);
                await _uow.SaveChangesAsync();

                return Result.Ok("تم الحذف بنجاح", ResultStatus.Success);
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
                    message: "حدث خطأ غير متوقع",
                    errorType: "DeleteFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
