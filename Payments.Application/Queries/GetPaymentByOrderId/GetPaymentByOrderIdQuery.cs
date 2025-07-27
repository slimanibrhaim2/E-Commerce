using MediatR;
using Core.Result;
using Payments.Application.DTOs;

namespace Payments.Application.Queries.GetPaymentByOrderId
{
    public record GetPaymentByOrderIdQuery(Guid OrderId) : IRequest<Result<PaymentWithMethodDTO>>;
} 