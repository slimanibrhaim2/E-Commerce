// Users.Application/Queries/GetAllUsers/GetAllUsersQueryHandler.cs
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Result;
using Users.Application.DTOs;
using Users.Domain.Repositories;
using Core.Pagination;

namespace Users.Application.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler
        : IRequestHandler<GetAllUsersQuery, Result<PaginatedResult<UserDTO>>>
    {
        private readonly IUserRepository _repo;

        public GetAllUsersQueryHandler(IUserRepository repo)
            => _repo = repo;

        public async Task<Result<PaginatedResult<UserDTO>>> Handle(
            GetAllUsersQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var users = (await _repo.GetAllAsync()).ToList();
                var totalCount = users.Count;
                var pageNumber = request.Parameters.PageNumber;
                var pageSize = request.Parameters.PageSize;
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var pagedUsers = users
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserDTO
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        MiddleName = u.MiddleName,
                        LastName = u.LastName,
                        PhoneNumber = u.PhoneNumber,
                        Email = u.Email,
                        ProfilePhoto = u.ProfilePhoto,
                        Description = u.Description,
                    })
                    .ToList();

                var paginatedResult = PaginatedResult<UserDTO>.Create(
                    data: pagedUsers,
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    totalCount: totalCount
                );

                return Result<PaginatedResult<UserDTO>>.Ok(
                    data: paginatedResult,
                    message: "تم استرجاع المستخدمين بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<UserDTO>>.Fail(
                    message: "حدث خطأ أثناء استرجاع قائمة المستخدمين",
                    errorType: "GetAllFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
