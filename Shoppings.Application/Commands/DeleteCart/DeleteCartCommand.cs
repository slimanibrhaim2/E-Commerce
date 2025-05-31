using MediatR;
using Core.Result;

namespace Shoppings.Application.Commands.DeleteCart
{
    public record DeleteCartCommand(Guid Id) : IRequest<Result<bool>>;
} 