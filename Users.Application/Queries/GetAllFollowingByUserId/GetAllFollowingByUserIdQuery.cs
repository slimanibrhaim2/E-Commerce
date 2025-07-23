using MediatR;
using System;
using Core.Result;
using Users.Application.DTOs;
using Core.Pagination;

namespace Users.Application.Queries.GetAllFollowingByUserId
{
    public record GetAllFollowingByUserIdQuery(Guid UserId, PaginationParameters Parameters) 
        : IRequest<Result<PaginatedResult<FollowingDTO>>>;
} 