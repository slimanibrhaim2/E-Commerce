using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using System.Collections.Generic;
using Core.Result;
using Core.Pagination;

namespace Shoppings.Application.Queries.GetAllOrderActivity
{
    public class GetAllOrderActivityQueryHandler : IRequestHandler<GetAllOrderActivityQuery, Result<PaginatedResult<OrderActivity>>>
    {
        private readonly IOrderActivityRepository _orderActivityRepository;
        public GetAllOrderActivityQueryHandler(IOrderActivityRepository orderActivityRepository)
        {
            _orderActivityRepository = orderActivityRepository;
        }

        public async Task<Result<PaginatedResult<OrderActivity>>> Handle(GetAllOrderActivityQuery request, CancellationToken cancellationToken)
        {
            var all = await _orderActivityRepository.GetAllAsync();
            var totalCount = all.Count();
            var data = all.Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                          .Take(request.Parameters.PageSize)
                          .ToList();
            var paginated = PaginatedResult<OrderActivity>.Create(data, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<OrderActivity>>.Ok(paginated, "تم جلب أنشطة الطلبات بنجاح", ResultStatus.Success);
        }
    }
} 