using MediatR;
using Core.Result;

namespace Payments.Application.Commands.CreatePaymentMethod
{
    public record CreatePaymentMethodCommand(string Name, string? Description, bool IsActive) : IRequest<Result<Guid>>;
} 