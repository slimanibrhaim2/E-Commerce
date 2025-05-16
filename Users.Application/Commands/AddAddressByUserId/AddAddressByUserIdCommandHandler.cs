// Users.Application/Commands/AddAddressByUserId/AddAddressByUserIdCommandHandler.cs
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Result;
using Users.Application.DTOs;
using Users.Domain.Entities;
using Users.Domain.Repositories;

namespace Users.Application.Commands.AddAddressByUserId
{
    public class AddAddressByUserIdCommandHandler
        : IRequestHandler<AddAddressByUserIdCommand, Result>
    {
        private readonly IUserRepository _repo;
        private readonly IUnitOfWork _uow;

        public AddAddressByUserIdCommandHandler(IUserRepository repo, IUnitOfWork uow)
            => (_repo, _uow) = (repo, uow);

        public async Task<Result> Handle(
            AddAddressByUserIdCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1. Load full user aggregate (with Addresses)
                var user = await _repo.GetByIdWithDetails(request.UserId);

                if(user is null)
                {
                    return Result.Fail(
                    message: "المستخدم غير موجود",
                    errorType: "user not found",
                    resultStatus: ResultStatus.Failed);
                }
                // 2. Create Address entity
                var address = new Address
                {
                    Id = Guid.NewGuid(),
                    Longitude = request.Longitude,
                    Latitude = request.Latitude,
                    Name = request.Name,
                    UserId = request.UserId
                };

                // 3. Add to user's Addresses
                user.Addresses ??= new List<Address>();
                user.Addresses.Add(address);

                // 4. Mark aggregate updated
                _repo.Update(user);

                // 5. Commit
                await _uow.SaveChangesAsync();

                return Result.Ok(
                    message: "تم إضافة العنوان بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                return Result.Fail(
                    message: "حدث خطأ أثناء إضافة العنوان",
                    errorType: "AddAddressFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
