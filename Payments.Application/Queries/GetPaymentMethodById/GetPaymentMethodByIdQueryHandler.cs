using MediatR;
using Core.Result;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;

namespace Payments.Application.Queries.GetPaymentMethodById
{
    public class GetPaymentMethodByIdQueryHandler : IRequestHandler<GetPaymentMethodByIdQuery, Result<PaymentMethod>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        public GetPaymentMethodByIdQueryHandler(IPaymentMethodRepository paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<Result<PaymentMethod>> Handle(GetPaymentMethodByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                return Result<PaymentMethod>.Fail("المعرف مطلوب.", "ValidationError", ResultStatus.ValidationError);
            var method = await _paymentMethodRepository.GetByIdAsync(request.Id);
            if (method == null)
                return Result<PaymentMethod>.Fail("طريقة الدفع غير موجودة.", "NotFound", ResultStatus.NotFound);
            return Result<PaymentMethod>.Ok(method, "تم جلب طريقة الدفع بنجاح", ResultStatus.Success);
        }
    }
} 