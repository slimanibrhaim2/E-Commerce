using MediatR;
using Catalogs.Domain.Repositories;
using Core.Result;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Catalogs.Domain.Entities;

namespace Catalogs.Application.Commands.UpdateCoupon
{
    public class UpdateCouponCommandHandler : IRequestHandler<UpdateCouponCommand, Result>
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateCouponCommandHandler(ICouponRepository couponRepository, IUnitOfWork unitOfWork)
        {
            _couponRepository = couponRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
        {
            Coupon? coupon = await _couponRepository.GetByIdAsync(request.Id);
            if (coupon == null)
                return Result.Fail("Coupon not found.", "NotFound", ResultStatus.NotFound);

            coupon.UserId = request.UserId;
            coupon.Code = request.Code;
            coupon.ExpiryDate = request.ExpiryDate;
            coupon.UpdatedAt = DateTime.UtcNow;

            _couponRepository.Update(coupon);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok("updated", ResultStatus.Success);
        }
    }
} 