using MediatR;
using Core.Result;
using Payments.Domain.Entities;

namespace Payments.Application.Queries.GetPaymentById
{
    public record GetPaymentByIdQuery(Guid Id) : IRequest<Result<Payment>>;
} 