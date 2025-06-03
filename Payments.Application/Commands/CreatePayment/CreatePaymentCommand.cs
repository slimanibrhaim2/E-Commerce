using MediatR;
using Core.Result;

namespace Payments.Application.Commands.CreatePayment
{
    public record CreatePaymentCommand(Guid OrderId, decimal Amount, Guid PaymentMethodId, Guid StatusId) : IRequest<Result<Guid>>;
} 