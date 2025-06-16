using MediatR;
using Core.Result;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;

namespace Payments.Application.Queries.GetPaymentById
{
    public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, Result<Payment>>
    {
        private readonly IPaymentRepository _paymentRepository;
        public GetPaymentByIdQueryHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<Result<Payment>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                return Result<Payment>.Fail("المعرف مطلوب.", "ValidationError", ResultStatus.ValidationError);
            var payment = await _paymentRepository.GetByIdAsync(request.Id);
            if (payment == null)
                return Result<Payment>.Fail("الدفع غير موجود.", "NotFound", ResultStatus.NotFound);
            return Result<Payment>.Ok(payment, "تم جلب الدفع بنجاح", ResultStatus.Success);
        }
    }
} 