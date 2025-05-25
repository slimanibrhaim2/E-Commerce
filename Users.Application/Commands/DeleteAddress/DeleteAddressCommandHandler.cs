using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Result;
using Core.Interfaces;
using Users.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq;
using Users.Domain.Entities;

namespace Users.Application.Commands.DeleteAddress
{
    public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, Result>
    {
        private readonly IAddressRepository _addressRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<DeleteAddressCommandHandler> _logger;

        public DeleteAddressCommandHandler(IAddressRepository addressRepo, IUnitOfWork uow, ILogger<DeleteAddressCommandHandler> logger)
            => (_addressRepo, _uow, _logger) = (addressRepo, uow, logger);

        public async Task<Result> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to soft delete address with ID: {AddressId}", request.AddressId);

                // Find the address
                IEnumerable<Address> addresses = await _addressRepo.FindAsync(a => a.Id == request.AddressId);
                Address? address = addresses.FirstOrDefault();
                if (address is null)
                {
                    _logger.LogWarning("Address not found with ID: {AddressId}", request.AddressId);
                    return Result.Fail(
                        message: "العنوان غير موجود",
                        errorType: "NotFound",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (address.DeletedAt != null)
                {
                    _logger.LogWarning("Address already deleted. AddressId: {AddressId}", request.AddressId);
                    return Result.Fail(
                        message: "العنوان محذوف بالفعل",
                        errorType: "AlreadyDeleted",
                        resultStatus: ResultStatus.ValidationError);
                }

                address.DeletedAt = DateTime.UtcNow;
                _addressRepo.Update(address);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("Successfully soft deleted address with ID: {AddressId}", request.AddressId);
                return Result.Ok(
                    message: "تم حذف العنوان بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft deleting address with ID: {AddressId}", request.AddressId);
                return Result.Fail(
                    message: "فشل في حذف العنوان",
                    errorType: "DeleteAddressFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 