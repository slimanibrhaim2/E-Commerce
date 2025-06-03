using MediatR;
using Core.Result;
using System;

namespace Catalogs.Application.Commands.DeleteCoupon
{
    public record DeleteCouponCommand(Guid Id) : IRequest<Result>;
} 