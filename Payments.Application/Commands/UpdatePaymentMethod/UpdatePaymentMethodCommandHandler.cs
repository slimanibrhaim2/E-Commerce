using MediatR;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;
using Core.Result;
using Payments.Application.Commands.UpdatePaymentMethod;
using Core.Interfaces;

namespace Payments.Application.Commands.Handlers
{
    public class UpdatePaymentMethodCommandHandler : IRequestHandler<UpdatePaymentMethodCommand, Result<bool>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdatePaymentMethodCommandHandler(IPaymentMethodRepository paymentMethodRepository, IUnitOfWork unitOfWork)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdatePaymentMethodCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<bool>.Fail("Name is required.", "ValidationError", ResultStatus.ValidationError);
            if (request.Name.Length > 100)
                return Result<bool>.Fail("Name cannot exceed 100 characters.", "ValidationError", ResultStatus.ValidationError);
            if (request.Description != null && request.Description.Length > 255)
                return Result<bool>.Fail("Description cannot exceed 255 characters.", "ValidationError", ResultStatus.ValidationError);
            var method = await _paymentMethodRepository.GetByIdAsync(request.Id);
            if (method == null)
                return Result<bool>.Fail("Payment method not found.", "NotFound", ResultStatus.NotFound);
            method.Name = request.Name;
            method.Description = request.Description;
            method.IsActive = request.IsActive;
            method.UpdatedAt = DateTime.UtcNow;
            _paymentMethodRepository.Update(method);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "updated", ResultStatus.Success);
        }
    }
} 