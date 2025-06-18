using MediatR;
using Catalogs.Application.DTOs;
using System;
using Core.Result;

namespace Catalogs.Application.Queries.GetCouponById
{
    public record GetCouponByIdQuery(Guid Id) : IRequest<Result<CouponDTO>>;
} 