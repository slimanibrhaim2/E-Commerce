using MediatR;
using Core.Result;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using Payments.Application.DTOs;

namespace Payments.Application.Queries.GetPaymentByOrderId
{
    public class GetPaymentByOrderIdQueryHandler : IRequestHandler<GetPaymentByOrderIdQuery, Result<PaymentWithMethodDTO>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        
        public GetPaymentByOrderIdQueryHandler(
            IPaymentRepository paymentRepository,
            IPaymentMethodRepository paymentMethodRepository)
        {
            _paymentRepository = paymentRepository;
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<Result<PaymentWithMethodDTO>> Handle(GetPaymentByOrderIdQuery request, CancellationToken cancellationToken)
        {
            if (request.OrderId == Guid.Empty)
                return Result<PaymentWithMethodDTO>.Fail("معرف الطلب مطلوب.", "ValidationError", ResultStatus.ValidationError);
            
            var payment = await _paymentRepository.GetByOrderIdAsync(request.OrderId);
            if (payment == null)
                return Result<PaymentWithMethodDTO>.Fail("لم يتم العثور على دفع لهذا الطلب.", "NotFound", ResultStatus.NotFound);
            
            // Get payment method name
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(payment.PaymentMethodId);
            string paymentMethodName = paymentMethod?.Name ?? "غير محدد";
            
            var result = new PaymentWithMethodDTO
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                PaymentMethodId = payment.PaymentMethodId,
                PaymentMethodName = paymentMethodName,
                StatusId = payment.StatusId,
                CreatedAt = payment.CreatedAt,
                UpdatedAt = payment.UpdatedAt
            };
            
            return Result<PaymentWithMethodDTO>.Ok(result, "تم جلب الدفع بنجاح", ResultStatus.Success);
        }
    }
} 