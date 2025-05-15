// Users.Application/Queries/GetAllFollowersByUserId/GetAllFollowersByUserIdQuery.cs
using MediatR;
using System;
using System.Collections.Generic;
using Core.Result;
using Users.Application.DTOs;

namespace Users.Application.Queries.GetAllFollowersByUserId
{
    public record GetAllFollowersByUserIdQuery(Guid UserId)
        : IRequest<Result<IEnumerable<FollowerDTO>>>;
}
