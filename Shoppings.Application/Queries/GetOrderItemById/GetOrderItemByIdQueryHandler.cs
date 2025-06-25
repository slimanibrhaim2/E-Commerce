using MediatR;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Repositories;
using Core.Result;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Shoppings.Application.Queries.GetOrderItemById
{
    public class GetOrderItemByIdQueryHandler : IRequestHandler<GetOrderItemByIdQuery, Result<OrderItemWithDetailsDTO>>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMediator _mediator;

        public GetOrderItemByIdQueryHandler(IOrderItemRepository orderItemRepository, IMediator mediator)
        {
            _orderItemRepository = orderItemRepository;
            _mediator = mediator;
        }

        public async Task<Result<OrderItemWithDetailsDTO>> Handle(GetOrderItemByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _orderItemRepository.GetByIdAsync(request.Id);
            if (entity == null)
                return Result<OrderItemWithDetailsDTO>.Fail("عنصر الطلب المطلوب غير موجود", "OrderItemNotFound", ResultStatus.NotFound);

            // Get item details using the new unified query
            var itemDetailsQuery = new GetItemDetailsByBaseItemIdQuery(entity.BaseItemId);
            var itemDetailsResult = await _mediator.Send(itemDetailsQuery, cancellationToken);

            // Determine the original ItemId (Product ID or Service ID) from the item details
            Guid itemId = entity.BaseItemId; // Default to BaseItemId if we can't resolve
            if (itemDetailsResult.Success && itemDetailsResult.Data != null)
            {
                itemId = itemDetailsResult.Data.Id; // This will be the Product ID or Service ID
            }

            // Create basic order item DTO
            var orderItemDto = new OrderItemDTO
            {
                Id = entity.Id,
                OrderId = entity.OrderId,
                BaseItemId = entity.BaseItemId,
                ItemId = itemId, // Set the original Product ID or Service ID
                Quantity = entity.Quantity,
                Price = entity.Price,
                CouponId = entity.CouponId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                DeletedAt = entity.DeletedAt
            };

            // Create composed DTO
            var composedDto = new OrderItemWithDetailsDTO
            {
                OrderItem = orderItemDto,
                ItemDetails = itemDetailsResult.Success ? itemDetailsResult.Data : null
            };

            return Result<OrderItemWithDetailsDTO>.Ok(composedDto, "تم جلب تفاصيل عنصر الطلب بنجاح", ResultStatus.Success);
        }
    }
} 