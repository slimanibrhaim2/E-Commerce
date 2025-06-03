using MediatR;
using Core.Result;
using Core.Pagination;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;

namespace Payments.Application.Queries.GetAllPayments
{
    public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, Result<PaginatedResult<Payment>>>
    {
        private readonly IPaymentRepository _paymentRepository;
        public GetAllPaymentsQueryHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<Result<PaginatedResult<Payment>>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
        {
            var all = await _paymentRepository.GetAllAsync();
            var totalCount = all.Count();
            var data = all.Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                          .Take(request.Parameters.PageSize)
                          .ToList();
            var paginated = PaginatedResult<Payment>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<Payment>>.Ok(paginated, "success", ResultStatus.Success);
        }
    }
} 