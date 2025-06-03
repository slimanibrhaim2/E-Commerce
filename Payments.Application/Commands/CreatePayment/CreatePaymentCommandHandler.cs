using MediatR;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using Core.Result;
using Payments.Application.Commands.CreatePayment;
using Core.Interfaces;

namespace Payments.Application.Commands.Handlers
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<Guid>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreatePaymentCommandHandler(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            if (request.OrderId == Guid.Empty)
                return Result<Guid>.Fail("OrderId is required.", "ValidationError", ResultStatus.ValidationError);
            if (request.PaymentMethodId == Guid.Empty)
                return Result<Guid>.Fail("PaymentMethodId is required.", "ValidationError", ResultStatus.ValidationError);
            if (request.StatusId == Guid.Empty)
                return Result<Guid>.Fail("StatusId is required.", "ValidationError", ResultStatus.ValidationError);
            if (request.Amount <= 0)
                return Result<Guid>.Fail("Amount must be greater than 0.", "ValidationError", ResultStatus.ValidationError);
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = request.OrderId,
                Amount = request.Amount,
                PaymentMethodId = request.PaymentMethodId,
                StatusId = request.StatusId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _paymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();
            return Result<Guid>.Ok(payment.Id, "added", ResultStatus.Success);
        }
    }
} 