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

namespace Users.Application.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler
        : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<UserDTO>>>
    {
        private readonly IUserRepository _repo;

        public GetAllUsersQueryHandler(IUserRepository repo)
            => _repo = repo;

        public async Task<Result<IEnumerable<UserDTO>>> Handle(
            GetAllUsersQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var users = await _repo.GetAllAsync();

                var dtoList = users.Select(u => new UserDTO
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
                .ToList()
                .AsEnumerable();

                return Result<IEnumerable<UserDTO>>.Ok(
                    data: dtoList,
                    message: "تم استرجاع المستخدمين بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<UserDTO>>.Fail(
                    message: ex.Message,
                    errorType: "GetAllFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
