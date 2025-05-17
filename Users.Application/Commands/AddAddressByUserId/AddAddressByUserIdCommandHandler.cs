// Users.Application/Commands/AddAddressByUserId/AddAddressByUserIdCommandHandler.cs
using MediatR;
using Core.Interfaces;
using Core.Result;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Users.Application.Commands.AddAddressByUserId
{
    public class AddAddressByUserIdCommandHandler
        : IRequestHandler<AddAddressByUserIdCommand, Result>
    {
        private readonly IUserRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AddAddressByUserIdCommandHandler> _logger;

        public AddAddressByUserIdCommandHandler(IUserRepository repo, IUnitOfWork uow, ILogger<AddAddressByUserIdCommandHandler> logger)
            => (_repo, _uow, _logger) = (repo, uow, logger);

        public async Task<Result> Handle(
            AddAddressByUserIdCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1. Load full user aggregate (with Addresses)
                var user = await _repo.GetByIdWithDetails(request.UserId);
                
                // Add logging here
                _logger.LogInformation($"User found: {user != null}");
                if (user != null)
                {
                    _logger.LogInformation($"User addresses count: {user.Addresses?.Count ?? 0}");
                }

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
                    Longitude = request.addressDTO.Longitude,
                    Latitude = request.addressDTO.Latitude,
                    Name = request.addressDTO.Name,
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
                // Add detailed logging
                _logger.LogError(ex, "Error adding address: {Message}", ex.Message);
                return Result.Fail(
                    message: "حدث خطأ أثناء إضافة العنوان",
                    errorType: "AddAddressFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
