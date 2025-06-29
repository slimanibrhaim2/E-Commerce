using MediatR;
using Core.Result;

namespace Shoppings.Application.Commands.CancelOrder;

public record CancelOrderCommand(
    Guid OrderId,
    Guid UserId
) : IRequest<Result<bool>>; 