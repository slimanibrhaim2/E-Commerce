using MediatR;
using Core.Result;
using Payments.Domain.Entities;

namespace Payments.Application.Queries.GetPaymentMethodById
{
    public record GetPaymentMethodByIdQuery(Guid Id) : IRequest<Result<PaymentMethod>>;
} 