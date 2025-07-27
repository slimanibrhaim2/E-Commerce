using MediatR;
using Core.Result;
using Payments.Domain.Repositories;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;

namespace Payments.Application.Queries.GetPaymentMethodByOrderId
{
    public class GetPaymentMethodByOrderIdQueryHandler : IRequestHandler<GetPaymentMethodByOrderIdQuery, Result<PaymentMethodInfoDTO>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IPaymentStatusRepository _paymentStatusRepository;
        
        public GetPaymentMethodByOrderIdQueryHandler(
            IPaymentRepository paymentRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IPaymentStatusRepository paymentStatusRepository)
        {
            _paymentRepository = paymentRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _paymentStatusRepository = paymentStatusRepository;
        }

        public async Task<Result<PaymentMethodInfoDTO>> Handle(GetPaymentMethodByOrderIdQuery request, CancellationToken cancellationToken)
        {
            if (request.OrderId == Guid.Empty)
                return Result<PaymentMethodInfoDTO>.Fail("معرف الطلب مطلوب.", "ValidationError", ResultStatus.ValidationError);
            
            var payment = await _paymentRepository.GetByOrderIdAsync(request.OrderId);
            if (payment == null)
                return Result<PaymentMethodInfoDTO>.Fail("لم يتم العثور على دفع لهذا الطلب.", "NotFound", ResultStatus.NotFound);
            
            // Get payment method name
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(payment.PaymentMethodId);
            string paymentMethodName = paymentMethod?.Name ?? "غير محدد";
            
            // Get payment status name
            var paymentStatus = await _paymentStatusRepository.GetByIdAsync(payment.StatusId);
            string paymentStatusName = paymentStatus?.Name ?? "غير محدد";
            
            var result = new PaymentMethodInfoDTO
            {
                PaymentId = payment.Id,
                OrderId = payment.OrderId,
                PaymentMethodName = paymentMethodName,
                Amount = payment.Amount,
                PaymentStatus = paymentStatusName
            };
            
            return Result<PaymentMethodInfoDTO>.Ok(result, "تم جلب معلومات الدفع بنجاح", ResultStatus.Success);
        }
    }
} 