using MediatR;
using Core.Result;

namespace Payments.Application.Commands.UpdatePaymentMethod
{
    public record UpdatePaymentMethodCommand(Guid Id, string Name, string? Description, bool IsActive) : IRequest<Result<bool>>;
} 