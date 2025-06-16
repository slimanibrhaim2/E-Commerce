using MediatR;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using Core.Result;
using Payments.Application.Commands.UpdatePayment;
using Core.Interfaces;

namespace Payments.Application.Commands.Handlers
{
    public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, Result<bool>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdatePaymentCommandHandler(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                return Result<bool>.Fail("المعرف مطلوب.", "ValidationError", ResultStatus.ValidationError);
            if (request.OrderId == Guid.Empty)
                return Result<bool>.Fail("معرف الطلب مطلوب.", "ValidationError", ResultStatus.ValidationError);
            if (request.PaymentMethodId == Guid.Empty)
                return Result<bool>.Fail("طريقة الدفع مطلوبة.", "ValidationError", ResultStatus.ValidationError);
            if (request.StatusId == Guid.Empty)
                return Result<bool>.Fail("حالة الدفع مطلوبة.", "ValidationError", ResultStatus.ValidationError);
            if (request.Amount <= 0)
                return Result<bool>.Fail("يجب أن يكون المبلغ أكبر من صفر.", "ValidationError", ResultStatus.ValidationError);
            var payment = await _paymentRepository.GetByIdAsync(request.Id);
            if (payment == null)
                return Result<bool>.Fail("الدفع غير موجود.", "NotFound", ResultStatus.NotFound);
            payment.OrderId = request.OrderId;
            payment.Amount = request.Amount;
            payment.PaymentMethodId = request.PaymentMethodId;
            payment.StatusId = request.StatusId;
            payment.UpdatedAt = DateTime.UtcNow;
            _paymentRepository.Update(payment);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "تم تحديث الدفع بنجاح", ResultStatus.Success);
        }
    }
} 