using MediatR;
using Core.Result;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;

namespace Payments.Application.Queries.GetPaymentStatusById
{
    public class GetPaymentStatusByIdQueryHandler : IRequestHandler<GetPaymentStatusByIdQuery, Result<PaymentStatus>>
    {
        private readonly IPaymentStatusRepository _paymentStatusRepository;
        public GetPaymentStatusByIdQueryHandler(IPaymentStatusRepository paymentStatusRepository)
        {
            _paymentStatusRepository = paymentStatusRepository;
        }

        public async Task<Result<PaymentStatus>> Handle(GetPaymentStatusByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                return Result<PaymentStatus>.Fail("المعرف مطلوب.", "ValidationError", ResultStatus.ValidationError);
            var status = await _paymentStatusRepository.GetByIdAsync(request.Id);
            if (status == null)
                return Result<PaymentStatus>.Fail("حالة الدفع غير موجودة.", "NotFound", ResultStatus.NotFound);
            return Result<PaymentStatus>.Ok(status, "تم جلب حالة الدفع بنجاح", ResultStatus.Success);
        }
    }
} 