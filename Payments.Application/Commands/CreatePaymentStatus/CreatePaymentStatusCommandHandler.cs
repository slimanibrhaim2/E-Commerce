using MediatR;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using Core.Result;
using Payments.Application.Commands.CreatePaymentStatus;
using Core.Interfaces;

namespace Payments.Application.Commands.Handlers
{
    public class CreatePaymentStatusCommandHandler : IRequestHandler<CreatePaymentStatusCommand, Result<Guid>>
    {
        private readonly IPaymentStatusRepository _paymentStatusRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreatePaymentStatusCommandHandler(IPaymentStatusRepository paymentStatusRepository, IUnitOfWork unitOfWork)
        {
            _paymentStatusRepository = paymentStatusRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreatePaymentStatusCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<Guid>.Fail("الاسم مطلوب.", "ValidationError", ResultStatus.ValidationError);
            if (request.Name.Length > 100)
                return Result<Guid>.Fail("لا يمكن أن يتجاوز الاسم 100 حرف.", "ValidationError", ResultStatus.ValidationError);
            var status = new PaymentStatus
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _paymentStatusRepository.AddAsync(status);
            await _unitOfWork.SaveChangesAsync();
            return Result<Guid>.Ok(status.Id, "تم إضافة حالة الدفع بنجاح", ResultStatus.Success);
        }
    }
} 