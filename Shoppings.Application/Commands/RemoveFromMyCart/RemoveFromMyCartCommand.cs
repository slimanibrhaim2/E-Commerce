using MediatR;
using Core.Result;

namespace Shoppings.Application.Commands.RemoveFromMyCart
{
    public record RemoveFromMyCartCommand(Guid UserId, Guid ItemId) : IRequest<Result>;
} 