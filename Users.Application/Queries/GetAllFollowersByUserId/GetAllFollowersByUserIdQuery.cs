// Users.Application/Queries/GetAllFollowersByUserId/GetAllFollowersByUserIdQuery.cs
using MediatR;
using System;
using System.Collections.Generic;
using Core.Result;
using Users.Application.DTOs;
using Core.Models;

namespace Users.Application.Queries.GetAllFollowersByUserId
{
    public record GetAllFollowersByUserIdQuery(Guid UserId, PaginationParameters Parameters)
        : IRequest<Result<PaginatedResult<FollowerDTO>>>;
}
