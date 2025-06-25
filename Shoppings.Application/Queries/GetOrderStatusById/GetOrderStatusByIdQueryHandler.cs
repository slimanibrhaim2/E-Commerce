using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Core.Result;

namespace Shoppings.Application.Queries.GetOrderStatusById
{
    public class GetOrderStatusByIdQueryHandler : IRequestHandler<GetOrderStatusByIdQuery, Result<OrderStatus>>
    {
        private readonly IOrderStatusRepository _orderStatusRepository;
        public GetOrderStatusByIdQueryHandler(IOrderStatusRepository orderStatusRepository)
        {
            _orderStatusRepository = orderStatusRepository;
        }

        public async Task<Result<OrderStatus>> Handle(GetOrderStatusByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _orderStatusRepository.GetByIdAsync(request.Id);
            if (entity == null)
                return Result<OrderStatus>.Fail("حالة الطلب غير موجودة", "OrderStatusNotFound", ResultStatus.NotFound);
            return Result<OrderStatus>.Ok(entity, "تم جلب حالة الطلب بنجاح", ResultStatus.Success);
        }
    }
} 