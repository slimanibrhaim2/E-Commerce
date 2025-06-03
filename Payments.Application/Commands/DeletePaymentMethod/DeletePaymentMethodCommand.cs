using MediatR;
using Core.Result;

namespace Payments.Application.Commands.DeletePaymentMethod
{
    public record DeletePaymentMethodCommand(Guid Id) : IRequest<Result<bool>>;
} 