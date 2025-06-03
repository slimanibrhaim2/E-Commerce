using MediatR;
using Core.Result;
using Core.Pagination;
using Payments.Domain.Entities;

namespace Payments.Application.Queries.GetAllPayments
{
    public record GetAllPaymentsQuery(PaginationParameters Parameters) : IRequest<Result<PaginatedResult<Payment>>>;
} 