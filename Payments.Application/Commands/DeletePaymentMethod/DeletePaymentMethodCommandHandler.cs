using MediatR;
using Payments.Domain.Repositories;
using Core.Result;
using Payments.Application.Commands.DeletePaymentMethod;
using Core.Interfaces;

namespace Payments.Application.Commands.Handlers
{
    public class DeletePaymentMethodCommandHandler : IRequestHandler<DeletePaymentMethodCommand, Result<bool>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeletePaymentMethodCommandHandler(IPaymentMethodRepository paymentMethodRepository, IUnitOfWork unitOfWork)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                return Result<bool>.Fail("المعرف مطلوب.", "ValidationError", ResultStatus.ValidationError);
            var method = await _paymentMethodRepository.GetByIdAsync(request.Id);
            if (method == null)
                return Result<bool>.Fail("طريقة الدفع غير موجودة.", "NotFound", ResultStatus.NotFound);
            _paymentMethodRepository.Remove(method);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "تم حذف طريقة الدفع بنجاح", ResultStatus.Success);
        }
    }
} 