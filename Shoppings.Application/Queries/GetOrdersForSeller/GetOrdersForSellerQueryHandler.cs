using Core.Result;
using Core.Pagination;
using MediatR;
using Microsoft.Extensions.Logging;
using Shoppings.Domain.Repositories;
using Shoppings.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;

namespace Shoppings.Application.Queries.GetOrdersForSeller;

public class GetOrdersForSellerQueryHandler : IRequestHandler<GetOrdersForSellerQuery, Result<PaginatedResult<SellerOrderDTO>>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusRepository _orderStatusRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<GetOrdersForSellerQueryHandler> _logger;

    public GetOrdersForSellerQueryHandler(
        IOrderRepository orderRepository,
        IOrderStatusRepository orderStatusRepository,
        IMediator mediator,
        ILogger<GetOrdersForSellerQueryHandler> logger)
    {
        _orderRepository = orderRepository;
        _orderStatusRepository = orderStatusRepository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<SellerOrderDTO>>> Handle(GetOrdersForSellerQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var orders = await _orderRepository.GetAllWithItemsAsync();
            var sellerOrders = new List<SellerOrderDTO>();

            // Get all order statuses to map status IDs to names
            var orderStatuses = await _orderStatusRepository.GetAllAsync();
            var statusDictionary = orderStatuses.ToDictionary(s => s.Id, s => s.Name);

            foreach (var order in orders)
            {
                var orderDTO = new SellerOrderDTO
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    OrderStatus = order.OrderActivity != null && statusDictionary.ContainsKey(order.OrderActivity.Status)
                        ? statusDictionary[order.OrderActivity.Status]
                        : string.Empty,
                    TotalAmount = order.TotalAmount,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt,
                    AddressId = order.AddressId,
                    Items = new List<OrderItemWithDetailsDTO>()
                };

                var hasSellerItems = false;

                foreach (var item in order.OrderItems)
                {
                    // First get the item ID (product or service) from base item ID
                    var itemIdResult = await _mediator.Send(
                        new GetItemIdByBaseItemIdQuery(item.BaseItemId),
                        cancellationToken
                    );

                    if (!itemIdResult.Success)
                    {
                        _logger.LogWarning("Failed to get item ID for BaseItemId {BaseItemId}: {Message}", item.BaseItemId, itemIdResult.Message);
                        continue;
                    }

                    // Get the user ID associated with this item from Catalogs module
                    var userIdResult = await _mediator.Send(
                        new GetUserIdByItemIdQuery(itemIdResult.Data.ItemId),
                        cancellationToken
                    );

                    if (!userIdResult.Success)
                    {
                        _logger.LogWarning("Failed to get user ID for ItemId {ItemId}: {Message}", itemIdResult.Data.ItemId, userIdResult.Message);
                        continue;
                    }

                    if (userIdResult.Data != request.SellerId)
                        continue;

                    // Get item details from Catalogs module
                    var itemDetailsResult = await _mediator.Send(
                        new GetItemDetailsByBaseItemIdQuery(item.BaseItemId),
                        cancellationToken
                    );

                    if (!itemDetailsResult.Success)
                    {
                        _logger.LogWarning("Failed to get item details for BaseItemId {BaseItemId}: {Message}", item.BaseItemId, itemDetailsResult.Message);
                        continue;
                    }

                    var orderItemDTO = new OrderItemWithDetailsDTO
                    {
                        Name = itemDetailsResult.Data.GetName(),
                        ImageUrl = itemDetailsResult.Data.GetImageUrl(),
                        Quantity = item.Quantity,
                        Price = item.Price,
                        TotalPrice = item.Price * item.Quantity,
                        OrderItem = new OrderItemDTO
                        {
                            ItemId = itemIdResult.Data.ItemId,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            TotalPrice = item.Price * item.Quantity
                        },
                        ItemDetails = itemDetailsResult.Data
                    };

                    orderDTO.Items.Add(orderItemDTO);
                    hasSellerItems = true;
                }

                if (hasSellerItems)
                {
                    sellerOrders.Add(orderDTO);
                }
            }

            var pageNumber = request.Parameters.PageNumber;
            var pageSize = request.Parameters.PageSize;
            var totalCount = sellerOrders.Count;
            var paged = sellerOrders
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginated = PaginatedResult<SellerOrderDTO>.Create(paged, pageNumber, pageSize, totalCount);

            return Result<PaginatedResult<SellerOrderDTO>>.Ok(
                data: paginated,
                message: totalCount > 0 ? "تم جلب الطلبات بنجاح" : "لا توجد طلبات لهذا البائع",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders for seller {SellerId}", request.SellerId);
            return Result<PaginatedResult<SellerOrderDTO>>.Fail(
                message: "حدث خطأ أثناء جلب الطلبات",
                errorType: "GetOrdersForSellerFailed",
                resultStatus: ResultStatus.Failed);
        }
    }
}