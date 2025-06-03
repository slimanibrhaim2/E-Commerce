using MediatR;
using Catalogs.Domain.Entities;
using Catalogs.Domain.Repositories;
using Core.Result;
using Catalogs.Application.Commands.CreateCoupon;
using Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Catalogs.Application.Commands.Handlers
{
    public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, Result<Guid>>
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateCouponCommandHandler(ICouponRepository couponRepository, IUnitOfWork unitOfWork)
        {
            _couponRepository = couponRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
                return Result<Guid>.Fail("Code is required.", "ValidationError", ResultStatus.ValidationError);
            var coupon = new Coupon
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Code = request.Code,
                DiscountAmount = request.DiscountAmount,
                ExpiryDate = request.ExpiryDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                DeletedAt = null
            };
            await _couponRepository.AddAsync(coupon);
            await _unitOfWork.SaveChangesAsync();
            // If CouponDAO generates an Id, you may need to fetch it after save
            return Result<Guid>.Ok(Guid.Empty, "added", ResultStatus.Success); // Replace Guid.Empty with actual Id if available
        }
    }
} 