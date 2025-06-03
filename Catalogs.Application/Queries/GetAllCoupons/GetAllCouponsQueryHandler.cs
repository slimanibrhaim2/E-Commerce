using MediatR;
using Catalogs.Domain.Repositories;
using Catalogs.Application.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Catalogs.Application.Queries.GetAllCoupons
{
    public class GetAllCouponsQueryHandler : IRequestHandler<GetAllCouponsQuery, List<CouponDTO>>
    {
        private readonly ICouponRepository _couponRepository;
        public GetAllCouponsQueryHandler(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        public async Task<List<CouponDTO>> Handle(GetAllCouponsQuery request, CancellationToken cancellationToken)
        {
            var coupons = await _couponRepository.GetAllAsync();
            return coupons.Select(c => new CouponDTO
            {
                Id = c.Id,
                UserId = c.UserId,
                Code = c.Code,
                DiscountAmount = c.DiscountAmount,
                ExpiryDate = c.ExpiryDate,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                DeletedAt = c.DeletedAt
            }).ToList();
        }
    }
} 