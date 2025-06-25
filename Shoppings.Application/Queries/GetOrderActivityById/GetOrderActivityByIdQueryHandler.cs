using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Core.Result;

namespace Shoppings.Application.Queries.GetOrderActivityById
{
    public class GetOrderActivityByIdQueryHandler : IRequestHandler<GetOrderActivityByIdQuery, Result<OrderActivity>>
    {
        private readonly IOrderActivityRepository _orderActivityRepository;
        public GetOrderActivityByIdQueryHandler(IOrderActivityRepository orderActivityRepository)
        {
            _orderActivityRepository = orderActivityRepository;
        }

        public async Task<Result<OrderActivity>> Handle(GetOrderActivityByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _orderActivityRepository.GetByIdAsync(request.Id);
            if (entity == null)
                return Result<OrderActivity>.Fail("نشاط الطلب غير موجود", "OrderActivityNotFound", ResultStatus.NotFound);
            return Result<OrderActivity>.Ok(entity, "تم جلب نشاط الطلب بنجاح", ResultStatus.Success);
        }
    }
} 