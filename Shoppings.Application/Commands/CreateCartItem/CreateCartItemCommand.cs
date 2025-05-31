using MediatR;
using Shoppings.Domain.Entities;
using Core.Result;

namespace Shoppings.Application.Commands.CreateCartItem
{
    public record CreateCartItemCommand(Guid CartId, Guid BaseItemId, int Quantity) : IRequest<Result<Guid>>;
} 