using MediatR;
using Core.Result;
using Users.Application.DTOs;
using System;

namespace Users.Application.Queries.GetAddressById;

public record GetAddressByIdQuery(Guid AddressId) : IRequest<Result<AddressDTO>>; 