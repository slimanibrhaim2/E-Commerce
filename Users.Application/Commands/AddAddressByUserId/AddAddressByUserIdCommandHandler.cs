// Users.Application/Commands/AddAddressByUserId/AddAddressByUserIdCommandHandler.cs
using MediatR;
using Core.Interfaces;
using Core.Result;
using Users.Domain.Entities;
using Users.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
                _logger.LogInformation("Attempting to add address for user with ID: {UserId}", request.UserId);

                // Validate UserId
                if (request.UserId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid UserId");
                    return Result.Fail(
                        message: "معرف المستخدم غير صالح",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Validate address properties
                if (string.IsNullOrWhiteSpace(request.addressDTO.Name))
                {
                    _logger.LogWarning("Invalid address name");
                    return Result.Fail(
                        message: "اسم العنوان مطلوب",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (request.addressDTO.Latitude < -90 || request.addressDTO.Latitude > 90)
                {
                    _logger.LogWarning("Invalid latitude value: {Latitude}", request.addressDTO.Latitude);
                    return Result.Fail(
                        message: "قيمة خط العرض غير صالحة",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (request.addressDTO.Longitude < -180 || request.addressDTO.Longitude > 180)
                {
                    _logger.LogWarning("Invalid longitude value: {Longitude}", request.addressDTO.Longitude);
                    return Result.Fail(
                        message: "قيمة خط الطول غير صالحة",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Load full user aggregate (with Addresses)
                var user = await _repo.GetByIdWithDetails(request.UserId);
                
                _logger.LogInformation("User found: {UserFound}, Addresses count: {AddressCount}", 
                    user != null, 
                    user?.Addresses?.Count ?? 0);

                if (user is null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", request.UserId);
                    return Result.Fail(
                        message: "المستخدم غير موجود",
                        errorType: "NotFound",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Check for duplicate address
                if (user.Addresses?.Any(a => 
                    a.Name.Equals(request.addressDTO.Name, StringComparison.OrdinalIgnoreCase) &&
                    a.Latitude == request.addressDTO.Latitude &&
                    a.Longitude == request.addressDTO.Longitude &&
                    a.DeletedAt == null) == true)
                {
                    _logger.LogWarning("Duplicate address found for user {UserId}", request.UserId);
                    return Result.Fail(
                        message: "هذا العنوان موجود بالفعل",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Create Address entity
                var address = new Address
                {
                    Id = Guid.NewGuid(),
                    Longitude = request.addressDTO.Longitude,
                    Latitude = request.addressDTO.Latitude,
                    Name = request.addressDTO.Name,
                    UserId = request.UserId
                };

                // Add to user's Addresses
                user.Addresses ??= new List<Address>();
                user.Addresses.Add(address);

                // Mark aggregate updated
                _repo.Update(user);

                // Commit
                await _uow.SaveChangesAsync();

                _logger.LogInformation("Successfully added address for user {UserId}", request.UserId);
                return Result.Ok(
                    message: "تم إضافة العنوان بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding address for user {UserId}: {Message}", 
                    request.UserId, ex.Message);
                return Result.Fail(
                    message: "حدث خطأ أثناء إضافة العنوان",
                    errorType: "AddAddressFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
