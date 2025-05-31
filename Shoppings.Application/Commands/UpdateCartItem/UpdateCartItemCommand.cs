using MediatR;
using Shoppings.Domain.Entities;
using Core.Result;

namespace Shoppings.Application.Commands.UpdateCartItem
{
    public record UpdateCartItemCommand(Guid Id, int Quantity) : IRequest<Result<bool>>;
} 