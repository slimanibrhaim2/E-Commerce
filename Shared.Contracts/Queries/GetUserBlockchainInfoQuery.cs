using Core.Result;
using MediatR;
using Shared.Contracts.DTOs.Blockchain;

namespace Shared.Contracts.Queries;

public record GetUserBlockchainInfoQuery(Guid UserId) : IRequest<Result<UserBlockchainDTO>>; 