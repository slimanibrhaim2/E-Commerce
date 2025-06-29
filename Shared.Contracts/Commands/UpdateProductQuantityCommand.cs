using MediatR;
using Core.Result;

namespace Shared.Contracts.Commands;

public record UpdateProductQuantityCommand(Guid ProductId, double QuantityChange) : IRequest<Result<bool>>; 