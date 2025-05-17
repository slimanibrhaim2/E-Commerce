// Users.Application/Queries/GetAddressesByUserId/GetAddressesByUserIdQuery.cs
using MediatR;
using System;
using System.Collections.Generic;
using Core.Result;
using Users.Application.DTOs;
using Core.Models;

namespace Users.Application.Queries.GetAddressesByUserId
{
    public record GetAddressesByUserIdQuery(Guid UserId, PaginationParameters Parameters)
        : IRequest<Result<PaginatedResult<AddressDTO>>>;
}
