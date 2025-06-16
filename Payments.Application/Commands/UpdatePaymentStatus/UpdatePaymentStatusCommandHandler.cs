using MediatR;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using Core.Result;
using Payments.Application.Commands.UpdatePaymentStatus;
using Core.Interfaces;

namespace Payments.Application.Commands.Handlers
{
    public class UpdatePaymentStatusCommandHandler : IRequestHandler<UpdatePaymentStatusCommand, Result<bool>>
    {
        private readonly IPaymentStatusRepository _paymentStatusRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdatePaymentStatusCommandHandler(IPaymentStatusRepository paymentStatusRepository, IUnitOfWork unitOfWork)
        {
            _paymentStatusRepository = paymentStatusRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<bool>.Fail("الاسم مطلوب.", "ValidationError", ResultStatus.ValidationError);
            if (request.Name.Length > 100)
                return Result<bool>.Fail("لا يمكن أن يتجاوز الاسم 100 حرف.", "ValidationError", ResultStatus.ValidationError);
            var status = await _paymentStatusRepository.GetByIdAsync(request.Id);
            if (status == null)
                return Result<bool>.Fail("حالة الدفع غير موجودة.", "NotFound", ResultStatus.NotFound);
            status.Name = request.Name;
            status.UpdatedAt = DateTime.UtcNow;
            _paymentStatusRepository.Update(status);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "تم تحديث حالة الدفع بنجاح", ResultStatus.Success);
        }
    }
} 