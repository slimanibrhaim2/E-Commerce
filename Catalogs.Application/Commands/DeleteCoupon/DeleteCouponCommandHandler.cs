using MediatR;
using Catalogs.Domain.Repositories;
using Core.Result;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Catalogs.Application.Commands.DeleteCoupon
{
    public class DeleteCouponCommandHandler : IRequestHandler<DeleteCouponCommand, Result>
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteCouponCommandHandler(ICouponRepository couponRepository, IUnitOfWork unitOfWork)
        {
            _couponRepository = couponRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
        {
            var coupon = await _couponRepository.GetByIdAsync(request.Id);
            if (coupon == null)
                return Result.Fail("Coupon not found.", "NotFound", ResultStatus.NotFound);

            _couponRepository.Remove(coupon);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok("deleted", ResultStatus.Success);
        }
    }
} 