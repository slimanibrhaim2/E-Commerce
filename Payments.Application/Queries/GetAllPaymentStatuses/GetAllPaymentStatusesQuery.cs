using MediatR;
using Core.Result;
using Core.Pagination;
using Payments.Domain.Entities;

namespace Payments.Application.Queries.GetAllPaymentStatuses
{
    public record GetAllPaymentStatusesQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<PaymentStatus>>>;
} 