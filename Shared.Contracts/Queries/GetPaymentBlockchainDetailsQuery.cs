using Core.Result;
using MediatR;
using Shared.Contracts.DTOs.Blockchain;

namespace Shared.Contracts.Queries;

public record GetPaymentBlockchainDetailsQuery(Guid PaymentId) : IRequest<Result<PaymentBlockchainDTO>>; 