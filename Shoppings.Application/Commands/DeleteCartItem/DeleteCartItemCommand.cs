using MediatR;
using Core.Result;

namespace Shoppings.Application.Commands
{
    public record DeleteCartItemCommand(Guid Id) : IRequest<Result<bool>>;
} 