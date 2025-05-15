// Users.Application/Queries/GetAddressesByUserId/GetAddressesByUserIdQuery.cs
using MediatR;
using System;
using System.Collections.Generic;
using Core.Result;
using Users.Application.DTOs;

namespace Users.Application.Queries.GetAddressesByUserId
{
    public record GetAddressesByUserIdQuery(Guid UserId)
        : IRequest<Result<IEnumerable<AddressDTO>>>;
}
