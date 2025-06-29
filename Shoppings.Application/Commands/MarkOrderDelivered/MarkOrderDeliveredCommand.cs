using Core.Result;
using MediatR;

namespace Shoppings.Application.Commands.MarkOrderDelivered;

public record MarkOrderDeliveredCommand(
    Guid OrderId,
    Guid UserId
) : IRequest<Result<bool>>; 