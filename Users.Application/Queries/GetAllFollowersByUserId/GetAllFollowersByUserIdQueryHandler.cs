// Users.Application/Queries/GetAllFollowersByUserId/GetAllFollowersByUserIdQueryHandler.cs
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Result;
using Users.Application.DTOs;
using Users.Domain.Repositories;

namespace Users.Application.Queries.GetAllFollowersByUserId
{
    public class GetAllFollowersByUserIdQueryHandler
        : IRequestHandler<GetAllFollowersByUserIdQuery, Result<IEnumerable<FollowerDTO>>>
    {
        private readonly IUserRepository _repo;

        public GetAllFollowersByUserIdQueryHandler(IUserRepository repo)
            => _repo = repo;

        public async Task<Result<IEnumerable<FollowerDTO>>> Handle(
            GetAllFollowersByUserIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1. Load followers directly from the repository
                var followers = (await _repo.GetFollowersByUserId(request.UserId))
                                    .ToList();

                // 2. If none found, verify the user actually exists
                if (!followers.Any())
                {
                    var userExists = await _repo.GetByIdWithDetails(request.UserId) != null;
                    if (!userExists)
                        throw new KeyNotFoundException(
                            $"User with Id {request.UserId} not found.");
                }

                // 3. Map to DTOs
                var dtos = followers
                    .Select(f => new FollowerDTO
                    {
                        Id = f.Id,
                        FollowerId = f.FollowerId,
                        FollowingId = f.FollowingId,
                        FollowedAt = f.FollowedAt
                    })
                    .AsEnumerable();

                // 4. Return success result
                return Result<IEnumerable<FollowerDTO>>.Ok(
                    data: dtos,
                    message: "تم جلب جميع المتابعين بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (KeyNotFoundException knf)
            {
                return Result<IEnumerable<FollowerDTO>>.Fail(
                    message: "غير موجود",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.ValidationError,
                    exception: knf);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<FollowerDTO>>.Fail(
                    message: "حدث خطأ أثناء جلب قائمة المتابعين",
                    errorType: "GetFollowersFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
