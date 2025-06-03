using MediatR;
using Core.Result;

namespace Payments.Application.Commands.DeletePayment
{
    public record DeletePaymentCommand(Guid Id) : IRequest<Result<bool>>;
} 