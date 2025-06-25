using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;

namespace Shoppings.Application.Queries.GetAllOrder
{
    public class GetAllOrderQueryHandler : IRequestHandler<GetAllOrderQuery, Result<PaginatedResult<Order>>>
    {
        private readonly IOrderRepository _orderRepository;
        public GetAllOrderQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Result<PaginatedResult<Order>>> Handle(GetAllOrderQuery request, CancellationToken cancellationToken)
        {
            var all = await _orderRepository.GetAllAsync();
            var totalCount = all.Count();
            var data = all.Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                          .Take(request.Parameters.PageSize)
                          .ToList();
            var paginated = PaginatedResult<Order>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<Order>>.Ok(paginated, "تم جلب الطلبات بنجاح", ResultStatus.Success);
        }
    }
} 