using Core.Result;
using Core.Pagination;
using MediatR;
using Shoppings.Domain.Repositories;
using Shoppings.Domain.Entities;

namespace Shoppings.Application.Queries.GetAllOrderStatus
{
    public class GetAllOrderStatusQueryHandler : IRequestHandler<GetAllOrderStatusQuery, Result<PaginatedResult<OrderStatus>>>
    {
        private readonly IOrderStatusRepository _orderStatusRepository;
        public GetAllOrderStatusQueryHandler(IOrderStatusRepository orderStatusRepository)
        {
            _orderStatusRepository = orderStatusRepository;
        }

        public async Task<Result<PaginatedResult<OrderStatus>>> Handle(GetAllOrderStatusQuery request, CancellationToken cancellationToken)
        {
            var all = await _orderStatusRepository.GetAllAsync();
            var totalCount = all.Count();
            var data = all.Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                          .Take(request.Parameters.PageSize)
                          .ToList();
            var paginated = PaginatedResult<OrderStatus>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<OrderStatus>>.Ok(paginated, "تم جلب حالات الطلبات بنجاح", ResultStatus.Success);
        }
    }
} 