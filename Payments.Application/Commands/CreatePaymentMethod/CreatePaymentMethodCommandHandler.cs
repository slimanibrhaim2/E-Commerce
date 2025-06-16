using MediatR;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using Core.Result;
using Payments.Application.Commands.CreatePaymentMethod;
using Core.Interfaces;

namespace Payments.Application.Commands.Handlers
{
    public class CreatePaymentMethodCommandHandler : IRequestHandler<CreatePaymentMethodCommand, Result<Guid>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreatePaymentMethodCommandHandler(IPaymentMethodRepository paymentMethodRepository, IUnitOfWork unitOfWork)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreatePaymentMethodCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<Guid>.Fail("الاسم مطلوب.", "ValidationError", ResultStatus.ValidationError);
            if (request.Name.Length > 100)
                return Result<Guid>.Fail("لا يمكن أن يتجاوز الاسم 100 حرف.", "ValidationError", ResultStatus.ValidationError);
            if (request.Description != null && request.Description.Length > 255)
                return Result<Guid>.Fail("لا يمكن أن يتجاوز الوصف 255 حرف.", "ValidationError", ResultStatus.ValidationError);
            var method = new PaymentMethod
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _paymentMethodRepository.AddAsync(method);
            await _unitOfWork.SaveChangesAsync();
            return Result<Guid>.Ok(method.Id, "تم إضافة طريقة الدفع بنجاح", ResultStatus.Success);
        }
    }
} 