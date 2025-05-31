using MediatR;
using Shoppings.Domain.Entities;
using Core.Result;

namespace Shoppings.Application.Commands
{
    public record UpdateOrderStatusCommand(Guid Id, string Name) : IRequest<Result<bool>>;
} 