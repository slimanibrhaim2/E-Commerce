using MediatR;
using Core.Result;
using Core.Pagination;
using Payments.Domain.Entities;

namespace Payments.Application.Queries.GetAllPaymentMethods
{
    public record GetAllPaymentMethodsQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<PaymentMethod>>>;
} 