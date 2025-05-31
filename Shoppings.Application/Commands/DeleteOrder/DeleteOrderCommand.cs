using MediatR;
using Core.Result;

namespace Shoppings.Application.Commands
{
    public record DeleteOrderCommand(Guid Id) : IRequest<Result<bool>>;
} 