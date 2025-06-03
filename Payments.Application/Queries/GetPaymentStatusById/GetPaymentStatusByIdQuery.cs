using MediatR;
using Core.Result;
using Payments.Domain.Entities;

namespace Payments.Application.Queries.GetPaymentStatusById
{
    public record GetPaymentStatusByIdQuery(Guid Id) : IRequest<Result<PaymentStatus>>;
} 