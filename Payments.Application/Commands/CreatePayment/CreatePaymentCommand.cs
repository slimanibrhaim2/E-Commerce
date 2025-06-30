using MediatR;
using Core.Result;

namespace Payments.Application.Commands.CreatePayment
{
    public record CreatePaymentCommand(Guid OrderId, double Amount, Guid PaymentMethodId, Guid StatusId) : IRequest<Result<Guid>>;
} 