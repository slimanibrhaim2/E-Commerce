using MediatR;
using Catalogs.Application.DTOs;
using System.Collections.Generic;
using Core.Result;

namespace Catalogs.Application.Queries.GetAllCoupons
{
    public record GetAllCouponsQuery() : IRequest<Result<List<CouponDTO>>>;
} 