using Core.Result;
using MediatR;
using Shared.Contracts.DTOs.Blockchain;

namespace Shared.Contracts.Queries;

public record GetOrderBlockchainDetailsQuery(Guid OrderId) : IRequest<Result<OrderBlockchainDTO>>; 