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

namespace Users.Application.Queries.GetAllFollowingByUserId
{
    public class GetAllFollowingByUserIdQueryHandler
        : IRequestHandler<GetAllFollowingByUserIdQuery, Result<PaginatedResult<FollowingDTO>>>
    {
        private readonly IFollowerRepository _followerRepo;
        private readonly IUserRepository _userRepo;

        public GetAllFollowingByUserIdQueryHandler(IFollowerRepository followerRepo, IUserRepository userRepo)
        {
            _followerRepo = followerRepo;
            _userRepo = userRepo;
        }

        public async Task<Result<PaginatedResult<FollowingDTO>>> Handle(
            GetAllFollowingByUserIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1. Load users that the current user follows with their details
                var following = (await _followerRepo.GetFollowingByUserId(request.UserId))
                                    .ToList();

                // 2. If none found, verify the user actually exists
                if (!following.Any())
                {
                    var userExists = await _userRepo.GetByIdWithDetails(request.UserId) != null;
                    if (!userExists)
                        throw new KeyNotFoundException(
                            $"User with Id {request.UserId} not found.");
                }

                // 3. Check follow relationships (should always be true since these are users the current user follows)
                var followingIds = following.Select(f => f.FollowingId).ToList();
                var followRelations = new HashSet<Guid>();
                
                foreach (var followingId in followingIds)
                {
                    var relation = await _followerRepo.GetByFollowerAndFollowingId(request.UserId, followingId);
                    if (relation != null)
                    {
                        followRelations.Add(followingId);
                    }
                }

                // 4. Map to DTOs with IsFollowed information
                var dtos = following
                    .Select(f => new FollowingDTO
                    {
                        FollowingId = f.FollowingId,
                        FollowingName = $"{f.Following?.FirstName} {f.Following?.LastName}".Trim(),
                        FollowingProfileUrl = f.Following?.ProfilePhoto ?? string.Empty,
                        CreatedAt = f.CreatedAt,
                        IsFollowed = followRelations.Contains(f.FollowingId) // Should always be true
                    })
                    .AsEnumerable();

                // 5. Create paginated result
                var paginatedResult = new PaginatedResult<FollowingDTO>
                {
                    Data = dtos,
                    PageNumber = request.Parameters.PageNumber,
                    PageSize = request.Parameters.PageSize,
                    TotalCount = dtos.Count(),
                    TotalPages = (int)Math.Ceiling(dtos.Count() / (double)request.Parameters.PageSize)
                };

                // 6. Return success result
                return Result<PaginatedResult<FollowingDTO>>.Ok(
                    data: paginatedResult,
                    message: "تم جلب جميع المتابَعين بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (KeyNotFoundException knf)
            {
                return Result<PaginatedResult<FollowingDTO>>.Fail(
                    message: "غير موجود",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.ValidationError,
                    exception: knf);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<FollowingDTO>>.Fail(
                    message: "حدث خطأ أثناء جلب قائمة المتابَعين",
                    errorType: "GetFollowingFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 