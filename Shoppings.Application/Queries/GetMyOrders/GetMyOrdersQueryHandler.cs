using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Application.DTOs;
using Shoppings.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;
using System.Linq;

namespace Shoppings.Application.Queries.GetMyOrders
{
    public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, Result<PaginatedResult<OrderItemDTO>>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<GetMyOrdersQueryHandler> _logger;

        public GetMyOrdersQueryHandler(
            IOrderRepository orderRepository,
            IMediator mediator,
            ILogger<GetMyOrdersQueryHandler> logger)
        {
            _orderRepository = orderRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Result<PaginatedResult<OrderItemDTO>>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var orders = await _orderRepository.GetAllByUserIdWithItemsAsync(request.UserId);
                var orderItems = new List<OrderItemDTO>();

                foreach (var order in orders)
                {
                    foreach (var item in order.OrderItems)
                    {
                        // Get item details from Catalogs module
                        var itemDetailsQuery = new GetItemDetailsByBaseItemIdQuery(item.BaseItemId);
                        var itemDetailsResult = await _mediator.Send(itemDetailsQuery, cancellationToken);

                        if (!itemDetailsResult.Success)
                            continue;

                        var itemDetails = itemDetailsResult.Data;
                        string name = GetItemName(itemDetails);
                        string imageUrl = GetImageUrl(itemDetails);

                        orderItems.Add(new OrderItemDTO
                        {
                            ItemId = itemDetails.Id,
                            ImageUrl = imageUrl,
                            Name = name,
                            Price = item.Price,
                            Quantity = (int)item.Quantity,
                            TotalPrice = item.Price * item.Quantity
                        });
                    }
                }

                // Apply pagination
                var totalCount = orderItems.Count;
                var paginatedItems = orderItems
                    .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                    .Take(request.Parameters.PageSize)
                    .ToList();

                var paginated = PaginatedResult<OrderItemDTO>.Create(
                    paginatedItems,
                    request.Parameters.PageNumber,
                    request.Parameters.PageSize,
                    totalCount);

                return Result<PaginatedResult<OrderItemDTO>>.Ok(
                    data: paginated,
                    message: "تم جلب الطلبات بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for user {UserId}", request.UserId);
                return Result<PaginatedResult<OrderItemDTO>>.Fail(
                    message: "فشل في جلب الطلبات",
                    errorType: "GetOrdersFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }

        private static string GetItemName(ItemDetailsDTO itemDetails)
        {
            return itemDetails switch
            {
                ProductDetailsDTO product => product.Name,
                ServiceDetailsDTO service => service.Name,
                _ => "عنصر غير معروف"
            };
        }

        private static string GetImageUrl(ItemDetailsDTO itemDetails)
        {
            if (itemDetails is ProductDetailsDTO product && product.Media != null && product.Media.Any())
            {
                return product.Media.FirstOrDefault()?.Url;
            }
            else if (itemDetails is ServiceDetailsDTO service && service.Media != null && service.Media.Any())
            {
                return service.Media.FirstOrDefault()?.Url;
            }
            return null;
        }
    }
} 