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
            Code = source.Code,
            DiscountAmount = (decimal)source.DiscountAmount,
            StartDate = source.CreatedAt,
            EndDate = source.ExpiryDate,
            IsActive = source.DeletedAt == null,
            // Map other properties as needed
        };
    }

    public CouponDAO MapBack(Coupon target)
    {
        if (target == null) return null;
        return new CouponDAO
        {
            Code = target.Code,
            DiscountAmount = (double)target.DiscountAmount,
            CreatedAt = target.StartDate,
            ExpiryDate = target.EndDate,
            // Set IsActive/DeletedAt as needed
        };
    }
} 