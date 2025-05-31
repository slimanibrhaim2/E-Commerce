using MediatR;
using Shoppings.Domain.Entities;
using Core.Result;

namespace Shoppings.Application.Commands.CreateCart
{
    public record CreateCartCommand(Guid UserId) : IRequest<Result<Guid>>;
} 