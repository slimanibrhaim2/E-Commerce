using MediatR;
using Catalogs.Domain.Repositories;
using Catalogs.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace Catalogs.Application.Queries.GetCouponById
{
    public class GetCouponByIdQueryHandler : IRequestHandler<GetCouponByIdQuery, CouponDTO?>
    {
        private readonly ICouponRepository _couponRepository;
        public GetCouponByIdQueryHandler(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        public async Task<CouponDTO?> Handle(GetCouponByIdQuery request, CancellationToken cancellationToken)
        {
            var c = await _couponRepository.GetByIdAsync(request.Id);
            if (c == null) return null;
            return new CouponDTO
            {
                Id = c.Id,
                UserId = c.UserId,
                Code = c.Code,
                DiscountAmount = c.DiscountAmount,
                ExpiryDate = c.ExpiryDate,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                DeletedAt = c.DeletedAt
            };
        }
    }
} 