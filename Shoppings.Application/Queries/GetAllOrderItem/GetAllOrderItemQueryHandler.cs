using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Repositories;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Shoppings.Application.Queries.GetAllOrderItem
{
    public class GetAllOrderItemQueryHandler : IRequestHandler<GetAllOrderItemQuery, Result<PaginatedResult<OrderItemWithDetailsDTO>>>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IMediator _mediator;

        public GetAllOrderItemQueryHandler(IOrderItemRepository orderItemRepository, IMediator mediator)
        {
            _orderItemRepository = orderItemRepository;
            _mediator = mediator;
        }

        public async Task<Result<PaginatedResult<OrderItemWithDetailsDTO>>> Handle(GetAllOrderItemQuery request, CancellationToken cancellationToken)
        {
            var all = await _orderItemRepository.GetAllAsync();
            var totalCount = all.Count();
            var paginatedOrderItems = all.Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                          .Take(request.Parameters.PageSize)
                          .ToList();

            // Create composed DTOs with item details
            var orderItemsWithDetails = new List<OrderItemWithDetailsDTO>();
            
            foreach (var orderItem in paginatedOrderItems)
            {
                // Get item details using the new unified query
                var itemDetailsQuery = new GetItemDetailsByBaseItemIdQuery(orderItem.BaseItemId);
                var itemDetailsResult = await _mediator.Send(itemDetailsQuery, cancellationToken);

                // Determine the original ItemId (Product ID or Service ID) from the item details
                Guid itemId = orderItem.BaseItemId; // Default to BaseItemId if we can't resolve
                if (itemDetailsResult.Success && itemDetailsResult.Data != null)
                {
                    itemId = itemDetailsResult.Data.Id; // This will be the Product ID or Service ID
                }

                var orderItemDto = new OrderItemDTO
                {
                    Id = orderItem.Id,
                    OrderId = orderItem.OrderId,
                    BaseItemId = orderItem.BaseItemId,
                    ItemId = itemId, // Set the original Product ID or Service ID
                    Quantity = orderItem.Quantity,
                    Price = orderItem.Price,
                    CouponId = orderItem.CouponId,
                    CreatedAt = orderItem.CreatedAt,
                    UpdatedAt = orderItem.UpdatedAt,
                    DeletedAt = orderItem.DeletedAt
                };

                var composedDto = new OrderItemWithDetailsDTO
                {
                    OrderItem = orderItemDto,
                    ItemDetails = itemDetailsResult.Success ? itemDetailsResult.Data : null
                };

                orderItemsWithDetails.Add(composedDto);
            }

            var paginated = PaginatedResult<OrderItemWithDetailsDTO>.Create(orderItemsWithDetails, request.Parameters.PageNumber, request.Parameters.PageSize, totalCount);
            return Result<PaginatedResult<OrderItemWithDetailsDTO>>.Ok(paginated, "تم جلب عناصر الطلبات بنجاح", ResultStatus.Success);
        }
    }
} 