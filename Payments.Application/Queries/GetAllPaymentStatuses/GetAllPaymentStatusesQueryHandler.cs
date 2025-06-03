using MediatR;
using Core.Result;
using Core.Pagination;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;

namespace Payments.Application.Queries.GetAllPaymentStatuses
{
    public class GetAllPaymentStatusesQueryHandler : IRequestHandler<GetAllPaymentStatusesQuery, Result<PaginatedResult<PaymentStatus>>>
    {
        private readonly IPaymentStatusRepository _paymentStatusRepository;
        public GetAllPaymentStatusesQueryHandler(IPaymentStatusRepository paymentStatusRepository)
        {
            _paymentStatusRepository = paymentStatusRepository;
        }

        public async Task<Result<PaginatedResult<PaymentStatus>>> Handle(GetAllPaymentStatusesQuery request, CancellationToken cancellationToken)
        {
            var all = await _paymentStatusRepository.GetAllAsync();
            var totalCount = all.Count();
            var data = all.Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                          .Take(request.Parameters.PageSize)
                          .ToList();
            var paginated = PaginatedResult<PaymentStatus>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<PaymentStatus>>.Ok(paginated, "success", ResultStatus.Success);
        }
    }
} 