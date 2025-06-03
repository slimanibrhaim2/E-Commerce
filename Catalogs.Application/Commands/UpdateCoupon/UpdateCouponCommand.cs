using MediatR;
using Core.Result;
using System;

namespace Catalogs.Application.Commands.UpdateCoupon
{
    public record UpdateCouponCommand(
        Guid Id,
        Guid UserId,
        string Code,
        double DiscountAmount,
        DateTime ExpiryDate
    ) : IRequest<Result>;
} 