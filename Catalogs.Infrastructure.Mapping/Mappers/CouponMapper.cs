using Infrastructure.Common;
using Infrastructure.Models;
using Catalogs.Domain.Entities;

namespace Catalogs.Infrastructure.Mapping.Mappers;

public class CouponMapper : IMapper<CouponDAO, Coupon>
{
    public Coupon Map(CouponDAO source)
    {
        if (source == null) return null;
        return new Coupon
        {
            Id = source.Id,
            UserId = source.UserId,
            Code = source.Code,
            DiscountAmount = source.DiscountAmount,
            ExpiryDate = source.ExpiryDate,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            DeletedAt = source.DeletedAt
        };
    }

    public CouponDAO MapBack(Coupon target)
    {
        if (target == null) return null;
        return new CouponDAO
        {
            Id = target.Id,
            UserId = target.UserId,
            Code = target.Code,
            DiscountAmount = target.DiscountAmount,
            ExpiryDate = target.ExpiryDate,
            CreatedAt = target.CreatedAt,
            UpdatedAt = target.UpdatedAt,
            DeletedAt = target.DeletedAt
        };
    }
} 