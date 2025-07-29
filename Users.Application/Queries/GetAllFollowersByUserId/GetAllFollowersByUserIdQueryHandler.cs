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
using Core.Pagination;

namespace Users.Application.Queries.GetAllFollowersByUserId
{
    public class GetAllFollowersByUserIdQueryHandler
        : IRequestHandler<GetAllFollowersByUserIdQuery, Result<PaginatedResult<FollowerDTO>>>
    {
        private readonly IFollowerRepository _followerRepo;
        private readonly IUserRepository _userRepo;

        public GetAllFollowersByUserIdQueryHandler(IFollowerRepository followerRepo, IUserRepository userRepo)
        {
            _followerRepo = followerRepo;
            _userRepo = userRepo;
        }

        public async Task<Result<PaginatedResult<FollowerDTO>>> Handle(
            GetAllFollowersByUserIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1. Load followers with their user details
                var followers = (await _followerRepo.GetFollowersByUserId(request.UserId))
                                    .ToList();

                // 2. If none found, verify the user actually exists
                if (!followers.Any())
                {
                    var userExists = await _userRepo.GetByIdWithDetails(request.UserId) != null;
                    if (!userExists)
                        throw new KeyNotFoundException(
                            $"User with Id {request.UserId} not found.");
                }

                // 3. Check which followers the current user follows back
                var followerIds = followers.Select(f => f.FollowerId).ToList();
                var followBackRelations = new HashSet<Guid>();
                
                foreach (var followerId in followerIds)
                {
                    var relation = await _followerRepo.GetByFollowerAndFollowingId(request.UserId, followerId);
                    if (relation != null)
                    {
                        followBackRelations.Add(followerId);
                    }
                }

                // 4. Map to DTOs with IsFollowed information
                var dtos = followers
                    .Select(f => new FollowerDTO
                    {
                        FollowerId = f.FollowerId,
                        FollowerName = $"{f.FollowerUser?.FirstName} {f.FollowerUser?.LastName}".Trim(),
                        FollowerProfileUrl = f.FollowerUser?.ProfilePhoto ?? string.Empty,
                        CreatedAt = f.CreatedAt,
                        IsFollowed = followBackRelations.Contains(f.FollowerId)
                    })
                    .AsEnumerable();

                // 5. Create paginated result
                var paginatedResult = new PaginatedResult<FollowerDTO>
                {
                    Data = dtos,
                    PageNumber = request.Parameters.PageNumber,
                    PageSize = request.Parameters.PageSize,
                    TotalCount = dtos.Count(),
                    TotalPages = (int)Math.Ceiling(dtos.Count() / (double)request.Parameters.PageSize)
                };

                // 6. Return success result
                return Result<PaginatedResult<FollowerDTO>>.Ok(
                    data: paginatedResult,
                    message: "تم جلب جميع المتابعين بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (KeyNotFoundException knf)
            {
                return Result<PaginatedResult<FollowerDTO>>.Fail(
                    message: "غير موجود",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.ValidationError,
                    exception: knf);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<FollowerDTO>>.Fail(
                    message: "حدث خطأ أثناء جلب قائمة المتابعين",
                    errorType: "GetFollowersFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
}
