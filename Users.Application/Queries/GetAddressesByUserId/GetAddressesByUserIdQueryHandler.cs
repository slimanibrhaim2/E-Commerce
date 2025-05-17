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
using Core.Models;

namespace Users.Application.Queries.GetAddressesByUserId
{
    public class GetAddressesByUserIdQueryHandler
        : IRequestHandler<GetAddressesByUserIdQuery, Result<PaginatedResult<AddressDTO>>>
    {
        private readonly IUserRepository _repo;

        public GetAddressesByUserIdQueryHandler(IUserRepository repo)
            => _repo = repo;

        public async Task<Result<PaginatedResult<AddressDTO>>> Handle(
            GetAddressesByUserIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var addresses = (await _repo.GetAddressesByUserId(request.UserId)).ToList();
                var totalCount = addresses.Count;
                var pageNumber = request.Parameters.PageNumber;
                var pageSize = request.Parameters.PageSize;
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var pagedAddresses = addresses
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new AddressDTO
                    {
                        Id = a.Id,
                        Latitude = a.Latitude,
                        Longitude = a.Longitude,
                        Name = a.Name
                    })
                    .ToList();

                var paginatedResult = new PaginatedResult<AddressDTO>
                {
                    Data = pagedAddresses,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalCount = totalCount
                };

                return Result<PaginatedResult<AddressDTO>>.Ok(
                    data: paginatedResult,
                    message: "تم جلب العناوين بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<AddressDTO>>.Fail(
                    message: "حدث خطأ أثناء جلب العناوين",
                    errorType: "GetAddressesFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
