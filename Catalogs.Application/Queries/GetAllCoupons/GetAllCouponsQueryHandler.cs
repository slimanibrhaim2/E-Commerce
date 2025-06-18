using MediatR;
using Catalogs.Domain.Repositories;
using Catalogs.Application.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Core.Result;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Queries.GetAllCoupons
{
    public class GetAllCouponsQueryHandler : IRequestHandler<GetAllCouponsQuery, Result<List<CouponDTO>>>
    {
        private readonly ICouponRepository _couponRepository;
        private readonly ILogger<GetAllCouponsQueryHandler> _logger;

        public GetAllCouponsQueryHandler(ICouponRepository couponRepository, ILogger<GetAllCouponsQueryHandler> logger)
        {
            _couponRepository = couponRepository;
            _logger = logger;
        }

        public async Task<Result<List<CouponDTO>>> Handle(GetAllCouponsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var coupons = await _couponRepository.GetAllAsync();
                var couponDtos = coupons.Select(c => new CouponDTO
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

                return Result<List<CouponDTO>>.Ok(
                    data: couponDtos,
                    message: "تم جلب الكوبونات بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving coupons");
                return Result<List<CouponDTO>>.Fail(
                    message: "فشل في جلب الكوبونات",
                    errorType: "GetAllCouponsFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 