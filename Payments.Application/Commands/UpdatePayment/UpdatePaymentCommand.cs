using MediatR;
using Core.Result;

namespace Payments.Application.Commands.UpdatePayment
{
    public record UpdatePaymentCommand(Guid Id, Guid OrderId, decimal Amount, Guid PaymentMethodId, Guid StatusId) : IRequest<Result<bool>>;
} 