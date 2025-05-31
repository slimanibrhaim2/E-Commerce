using MediatR;
using Shoppings.Domain.Entities;
using Core.Result;

namespace Shoppings.Application.Commands.UpdateCart
{
    public record UpdateCartCommand(Guid Id, Guid UserId) : IRequest<Result<bool>>;
} 