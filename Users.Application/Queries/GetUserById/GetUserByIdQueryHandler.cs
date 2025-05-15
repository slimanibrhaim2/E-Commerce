// Users.Application/Queries/GetUserById/GetUserByIdQueryHandler.cs
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Users.Application.DTOs;
using Users.Domain.Repositories;
using Core.Result;

namespace Users.Application.Queries.GetUserById
{
    public class GetUserByIdQueryHandler
        : IRequestHandler<GetUserByIdQuery, Result<UserDTO>>
    {
        private readonly IUserRepository _repo;

        public GetUserByIdQueryHandler(IUserRepository repo)
            => _repo = repo;

        public async Task<Result<UserDTO>> Handle(
            GetUserByIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Use the repository method that includes details
                var user = await _repo.GetByIdWithDetails(request.Id)
                           ?? throw new KeyNotFoundException($"User with Id {request.Id} not found.");

                var dto = new UserDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    ProfilePhoto = user.ProfilePhoto,
                    Description = user.Description,
                    
                };

                return Result<UserDTO>.Ok(
                    data: dto,
                    message: "تم جلب بيانات المستخدم بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (KeyNotFoundException knf)
            {
                return Result<UserDTO>.Fail(
                    message: knf.Message,
                    errorType: "NotFound",
                    resultStatus: ResultStatus.ValidationError,
                    exception: knf);
            }
            catch (Exception ex)
            {
                return Result<UserDTO>.Fail(
                    message: "حدث خطأ أثناء جلب بيانات المستخدم",
                    errorType: "GetByIdFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
