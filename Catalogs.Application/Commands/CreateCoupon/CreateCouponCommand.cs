using MediatR;
using Core.Result;
using System;
using System.Collections.Generic;
using Catalogs.Domain.Entities;

namespace Catalogs.Application.Commands.CreateCoupon
{
    public record CreateCouponCommand(
        Guid UserId,
        string Code,
        double DiscountAmount,
        DateTime ExpiryDate
    ) : IRequest<Result<Guid>>;
} 