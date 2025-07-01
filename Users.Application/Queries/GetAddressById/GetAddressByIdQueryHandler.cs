using MediatR;
using Core.Result;
using Users.Application.DTOs;
using Users.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Users.Application.Queries.GetAddressById;

public class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, Result<AddressDTO>>
{
    private readonly IAddressRepository _addressRepository;
    private readonly ILogger<GetAddressByIdQueryHandler> _logger;

    public GetAddressByIdQueryHandler(IAddressRepository addressRepository, ILogger<GetAddressByIdQueryHandler> logger)
    {
        _addressRepository = addressRepository;
        _logger = logger;
    }

    public async Task<Result<AddressDTO>> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Attempting to get address with ID: {AddressId}", request.AddressId);

            var address = await _addressRepository.GetByIdAsync(request.AddressId);
            if (address == null)
            {
                _logger.LogWarning("Address not found with ID: {AddressId}", request.AddressId);
                return Result<AddressDTO>.Fail(
                    message: "العنوان غير موجود",
                    errorType: "AddressNotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            var addressDto = new AddressDTO
            {
                Id = address.Id,
                Name = address.Name,
                Latitude = address.Latitude,
                Longitude = address.Longitude
            };

            return Result<AddressDTO>.Ok(
                data: addressDto,
                message: "تم جلب العنوان بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting address with ID: {AddressId}", request.AddressId);
            return Result<AddressDTO>.Fail(
                message: $"فشل في جلب العنوان: {ex.Message}",
                errorType: "GetAddressByIdFailed",
                resultStatus: ResultStatus.Failed,
                exception: ex);
        }
    }
} 