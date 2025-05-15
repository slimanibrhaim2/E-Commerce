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
                var user = await _repo.GetByIdWithDetails(request.UserId)
                           ?? throw new KeyNotFoundException(
                               $"User with Id {request.UserId} not found.");

                // 2. Create Address entity
                var dto = request.Address;
                var address = new Address
                {
                    Id = Guid.NewGuid(),
                    Longitude = dto.Longitude,
                    Latitude = dto.Latitude,
                    Name = dto.Name,
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
            catch (KeyNotFoundException knf)
            {
                return Result.Fail(
                    message: knf.Message,
                    errorType: "NotFound",
                    resultStatus: ResultStatus.ValidationError,
                    exception: knf);
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
