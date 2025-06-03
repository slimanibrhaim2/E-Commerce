using MediatR;
using Catalogs.Application.DTOs;
using System;

namespace Catalogs.Application.Queries.GetCouponById
{
    public record GetCouponByIdQuery(Guid Id) : IRequest<CouponDTO?>;
} 