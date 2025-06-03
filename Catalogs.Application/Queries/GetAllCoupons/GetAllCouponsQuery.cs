using MediatR;
using Catalogs.Application.DTOs;
using System.Collections.Generic;

namespace Catalogs.Application.Queries.GetAllCoupons
{
    public record GetAllCouponsQuery() : IRequest<List<CouponDTO>>;
} 