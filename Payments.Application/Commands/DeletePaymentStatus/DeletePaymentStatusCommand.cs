using MediatR;
using Core.Result;

namespace Payments.Application.Commands.DeletePaymentStatus
{
    public record DeletePaymentStatusCommand(Guid Id) : IRequest<Result<bool>>;
} 