using MediatR;
using Core.Result;

namespace Payments.Application.Commands.CreatePaymentStatus
{
    public record CreatePaymentStatusCommand(string Name) : IRequest<Result<Guid>>;
} 