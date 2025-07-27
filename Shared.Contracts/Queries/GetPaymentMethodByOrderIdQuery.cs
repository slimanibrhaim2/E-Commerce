using Core.Result;
using MediatR;
using Shared.Contracts.DTOs;

namespace Shared.Contracts.Queries;

public record GetPaymentMethodByOrderIdQuery(Guid OrderId) : IRequest<Result<PaymentMethodInfoDTO>>; 