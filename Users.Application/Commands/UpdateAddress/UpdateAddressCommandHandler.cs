using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Result;
using Core.Interfaces;
using Users.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Users.Domain.Entities;

namespace Users.Application.Commands.UpdateAddress
{
    public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, Result>
    {
        private readonly IAddressRepository _addressRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<UpdateAddressCommandHandler> _logger;

        public UpdateAddressCommandHandler(IAddressRepository addressRepo, IUnitOfWork uow, ILogger<UpdateAddressCommandHandler> logger)
            => (_addressRepo, _uow, _logger) = (addressRepo, uow, logger);

        public async Task<Result> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to update address with ID: {AddressId}", request.AddressId);

                // Validation
                if (request.AddressId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid AddressId");
                    return Result.Fail(
                        message: "معرف العنوان غير صالح",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (string.IsNullOrWhiteSpace(request.AddressDTO.Name))
                {
                    _logger.LogWarning("Invalid Address Name");
                    return Result.Fail(
                        message: "يرجى إدخال اسم العنوان",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Find the address
                var address = await _addressRepo.GetByIdAsync(request.AddressId);
                if (address is null)
                {
                    _logger.LogWarning("Address not found with ID: {AddressId}", request.AddressId);
                    return Result.Fail(
                        message: "العنوان غير موجود",
                        errorType: "NotFound",
                        resultStatus: ResultStatus.NotFound);
                }

                // Check if address is deleted
                if (address.DeletedAt != null)
                {
                    _logger.LogWarning("Address is deleted. AddressId: {AddressId}", request.AddressId);
                    return Result.Fail(
                        message: "لا يمكن تحديث عنوان محذوف",
                        errorType: "AddressDeleted",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Update address properties
                address.Name = request.AddressDTO.Name;
                address.Latitude = request.AddressDTO.Latitude;
                address.Longitude = request.AddressDTO.Longitude;

                _addressRepo.Update(address);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("Successfully updated address with ID: {AddressId}", request.AddressId);
                return Result.Ok(
                    message: "تم تحديث العنوان بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address with ID: {AddressId}", request.AddressId);
                return Result.Fail(
                    message: "فشل في تحديث العنوان",
                    errorType: "UpdateAddressFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 