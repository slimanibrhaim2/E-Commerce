using MediatR;
using Catalogs.Domain.Repositories;
using Catalogs.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;
using Core.Result;
using Microsoft.Extensions.Logging;

namespace Catalogs.Application.Queries.GetCouponById
{
    public class GetCouponByIdQueryHandler : IRequestHandler<GetCouponByIdQuery, Result<CouponDTO>>
    {
        private readonly ICouponRepository _couponRepository;
        private readonly ILogger<GetCouponByIdQueryHandler> _logger;

        public GetCouponByIdQueryHandler(ICouponRepository couponRepository, ILogger<GetCouponByIdQueryHandler> logger)
        {
            _couponRepository = couponRepository;
            _logger = logger;
        }

        public async Task<Result<CouponDTO>> Handle(GetCouponByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Id == Guid.Empty)
                {
                    return Result<CouponDTO>.Fail(
                        message: "معرف الكوبون مطلوب",
                        errorType: "ValidationError",
                        resultStatus: ResultStatus.ValidationError);
                }

                var c = await _couponRepository.GetByIdAsync(request.Id);
                if (c == null)
                {
                    return Result<CouponDTO>.Fail(
                        message: "الكوبون غير موجود",
                        errorType: "CouponNotFound",
                        resultStatus: ResultStatus.NotFound);
                }

                var couponDto = new CouponDTO
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

                return Result<CouponDTO>.Ok(
                    data: couponDto,
                    message: "تم جلب الكوبون بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving coupon with ID {CouponId}", request.Id);
                return Result<CouponDTO>.Fail(
                    message: "فشل في جلب الكوبون",
                    errorType: "GetCouponByIdFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 