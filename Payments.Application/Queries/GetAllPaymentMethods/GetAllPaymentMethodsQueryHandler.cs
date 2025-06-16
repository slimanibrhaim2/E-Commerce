using MediatR;
using Core.Result;
using Core.Pagination;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;

namespace Payments.Application.Queries.GetAllPaymentMethods
{
    public class GetAllPaymentMethodsQueryHandler : IRequestHandler<GetAllPaymentMethodsQuery, Result<PaginatedResult<PaymentMethod>>>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        public GetAllPaymentMethodsQueryHandler(IPaymentMethodRepository paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<Result<PaginatedResult<PaymentMethod>>> Handle(GetAllPaymentMethodsQuery request, CancellationToken cancellationToken)
        {
            var all = await _paymentMethodRepository.GetAllAsync();
            var totalCount = all.Count();
            var data = all.Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                          .Take(request.Parameters.PageSize)
                          .ToList();
            var paginated = PaginatedResult<PaymentMethod>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<PaymentMethod>>.Ok(paginated, "تم جلب طرق الدفع بنجاح", ResultStatus.Success);
        }
    }
} 