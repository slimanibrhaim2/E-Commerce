// Users.Application/Queries/GetAddressesByUserId/GetAddressesByUserIdQueryHandler.cs
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Result;
using Users.Application.DTOs;
using Users.Domain.Repositories;

namespace Users.Application.Queries.GetAddressesByUserId
{
    public class GetAddressesByUserIdQueryHandler
        : IRequestHandler<GetAddressesByUserIdQuery, Result<IEnumerable<AddressDTO>>>
    {
        private readonly IUserRepository _repo;

        public GetAddressesByUserIdQueryHandler(IUserRepository repo)
            => _repo = repo;

        public async Task<Result<IEnumerable<AddressDTO>>> Handle(
            GetAddressesByUserIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1. Load addresses directly
                var addresses = await _repo.GetAddressesByUserId(request.UserId);

                // 2. If empty and user might not exist, we can check existence:
                if (!addresses.Any())
                {
                    // Optionally verify user exists:
                    var userExists = await _repo.GetByIdWithDetails(request.UserId) != null;
                    if (!userExists)
                        throw new KeyNotFoundException($"User with Id {request.UserId} not found.");
                }

                // 3. Map to DTOs
                var dtos = addresses
                    .Select(a => new AddressDTO
                    {
                        Id = a.Id,
                        Latitude = a.Latitude,
                        Longitude = a.Longitude,
                        Name = a.Name
                    })
                    .ToList()
                    .AsEnumerable();

                // 4. Return success
                return Result<IEnumerable<AddressDTO>>.Ok(
                    data: dtos,
                    message: "تم جلب العناوين بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (KeyNotFoundException knf)
            {
                return Result<IEnumerable<AddressDTO>>.Fail(
                    message: knf.Message,
                    errorType: "NotFound",
                    resultStatus: ResultStatus.ValidationError,
                    exception: knf);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<AddressDTO>>.Fail(
                    message: "حدث خطأ أثناء جلب العناوين",
                    errorType: "GetAddressesFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
