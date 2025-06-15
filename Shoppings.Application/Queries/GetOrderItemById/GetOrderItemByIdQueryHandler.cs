using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Core.Result;

namespace Shoppings.Application.Queries.GetOrderItemById
{
    public class GetOrderItemByIdQueryHandler : IRequestHandler<GetOrderItemByIdQuery, Result<OrderItem>>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        public GetOrderItemByIdQueryHandler(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        public async Task<Result<OrderItem>> Handle(GetOrderItemByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _orderItemRepository.GetByIdAsync(request.Id);
            if (entity == null)
                return Result<OrderItem>.Fail("عنصر الطلب غير موجود", "NotFound", ResultStatus.NotFound);
            return Result<OrderItem>.Ok(entity, "تم جلب عنصر الطلب بنجاح", ResultStatus.Success);
        }
    }
} 