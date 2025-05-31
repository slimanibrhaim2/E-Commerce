using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;

namespace Shoppings.Application.Queries.GetAllOrderItem
{
    public class GetAllOrderItemQueryHandler : IRequestHandler<GetAllOrderItemQuery, Result<PaginatedResult<OrderItem>>>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        public GetAllOrderItemQueryHandler(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        public async Task<Result<PaginatedResult<OrderItem>>> Handle(GetAllOrderItemQuery request, CancellationToken cancellationToken)
        {
            var all = await _orderItemRepository.GetAllAsync();
            var totalCount = all.Count();
            var data = all.Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                          .Take(request.Parameters.PageSize)
                          .ToList();
            var paginated = PaginatedResult<OrderItem>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<OrderItem>>.Ok(paginated, "success", ResultStatus.Success);
        }
    }
} 