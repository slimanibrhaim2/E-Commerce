using Core.Result;
using MediatR;

namespace Shared.Contracts.Commands;

public record PayOrderCommand(Guid OrderId) : IRequest<Result>; 