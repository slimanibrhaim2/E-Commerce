using MediatR;
using Core.Result;

namespace Shoppings.Application.Commands.UpdateMyCartItem
{
    public record UpdateMyCartItemCommand(Guid UserId, Guid ItemId, int Quantity) : IRequest<Result<bool>>;
} 