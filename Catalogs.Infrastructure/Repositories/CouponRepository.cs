using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;

namespace Catalogs.Infrastructure.Repositories;

public class CouponRepository : BaseRepository<Coupon, CouponDAO>, ICouponRepository
{
    public CouponRepository(ECommerceContext context, IMapper<CouponDAO, Coupon> mapper)
        : base(context, mapper)
    {
    }
} 