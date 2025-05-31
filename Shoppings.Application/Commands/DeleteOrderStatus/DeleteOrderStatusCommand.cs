using MediatR;
using Core.Result;

namespace Shoppings.Application.Commands.DeleteOrderStatus
{
    public record DeleteOrderStatusCommand(Guid Id) : IRequest<Result<bool>>;
} 