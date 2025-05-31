using MediatR;
using Core.Result;

namespace Shoppings.Application.Commands
{
    public record DeleteOrderActivityCommand(Guid Id) : IRequest<Result<bool>>;
} 