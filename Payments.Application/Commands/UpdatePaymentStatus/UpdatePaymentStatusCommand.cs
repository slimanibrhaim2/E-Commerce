using MediatR;
using Core.Result;

namespace Payments.Application.Commands.UpdatePaymentStatus
{
    public record UpdatePaymentStatusCommand(Guid Id, string Name) : IRequest<Result<bool>>;
} 