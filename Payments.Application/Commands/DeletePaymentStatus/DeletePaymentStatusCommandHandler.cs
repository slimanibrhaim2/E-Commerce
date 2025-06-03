using MediatR;
using Payments.Domain.Repositories;
using Core.Result;
using Payments.Application.Commands.DeletePaymentStatus;
using Core.Interfaces;

namespace Payments.Application.Commands.Handlers
{
    public class DeletePaymentStatusCommandHandler : IRequestHandler<DeletePaymentStatusCommand, Result<bool>>
    {
        private readonly IPaymentStatusRepository _paymentStatusRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeletePaymentStatusCommandHandler(IPaymentStatusRepository paymentStatusRepository, IUnitOfWork unitOfWork)
        {
            _paymentStatusRepository = paymentStatusRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeletePaymentStatusCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
                return Result<bool>.Fail("Id is required.", "ValidationError", ResultStatus.ValidationError);
            var status = await _paymentStatusRepository.GetByIdAsync(request.Id);
            if (status == null)
                return Result<bool>.Fail("Payment status not found.", "NotFound", ResultStatus.NotFound);
            _paymentStatusRepository.Remove(status);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "deleted", ResultStatus.Success);
        }
    }
} 