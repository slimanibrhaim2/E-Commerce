using MediatR;
using Payments.Domain.Repositories;
using Core.Result;
using Payments.Application.Commands.DeletePayment;
using Core.Interfaces;

namespace Payments.Application.Commands.Handlers
{
    public class DeletePaymentCommandHandler : IRequestHandler<DeletePaymentCommand, Result<bool>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeletePaymentCommandHandler(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                return Result<bool>.Fail("Id is required.", "ValidationError", ResultStatus.ValidationError);
            var payment = await _paymentRepository.GetByIdAsync(request.Id);
            if (payment == null)
                return Result<bool>.Fail("Payment not found.", "NotFound", ResultStatus.NotFound);
            _paymentRepository.Remove(payment);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "deleted", ResultStatus.Success);
        }
    }
} 