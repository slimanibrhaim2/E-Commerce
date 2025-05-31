using MediatR;
using Shoppings.Domain.Entities;
using Core.Result;

namespace Shoppings.Application.Commands
{
    public record UpdateOrderActivityCommand(Guid Id, Guid Status) : IRequest<Result<bool>>;
} 