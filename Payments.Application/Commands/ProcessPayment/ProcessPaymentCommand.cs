using Core.Result;
using MediatR;
using Payments.Domain.Entities;

namespace Payments.Application.Commands.ProcessPayment;

public record ProcessPaymentCommand(
    Guid OrderId,
    double Amount,
    Guid PaymentMethodId,
    string? PaymentDetails = null) : IRequest<Result<Payment>>; 