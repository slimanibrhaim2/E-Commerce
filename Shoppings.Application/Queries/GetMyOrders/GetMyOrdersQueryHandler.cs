using MediatR;
using Core.Result;
using Core.Pagination;
using Shoppings.Domain.Repositories;
using Shoppings.Application.DTOs;
using Microsoft.Extensions.Logging;
using System.Linq;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;

namespace Shoppings.Application.Queries.GetMyOrders;

public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, Result<PaginatedResult<MyOrderDTO>>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusRepository _orderStatusRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<GetMyOrdersQueryHandler> _logger;

    public GetMyOrdersQueryHandler(
        IOrderRepository orderRepository,
        IOrderStatusRepository orderStatusRepository,
        IMediator mediator,
        ILogger<GetMyOrdersQueryHandler> logger)
    {
        _orderRepository = orderRepository;
        _orderStatusRepository = orderStatusRepository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Result<PaginatedResult<MyOrderDTO>>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get all orders for the user with their items
            var orders = await _orderRepository.GetAllByUserIdWithItemsAsync(request.UserId);
            if (!orders.Any())
            {
                return Result<PaginatedResult<MyOrderDTO>>.Ok(
                    data: PaginatedResult<MyOrderDTO>.Create(
                        Enumerable.Empty<MyOrderDTO>(),
                        request.Parameters.PageNumber,
                        request.Parameters.PageSize,
                        0),
                    message: "لا توجد طلبات",
                    resultStatus: ResultStatus.Success);
            }

            // Get all order statuses to map status IDs to names
            var orderStatuses = await _orderStatusRepository.GetAllAsync();
            var statusDictionary = orderStatuses.ToDictionary(s => s.Id, s => s.Name);

            // Transform orders into DTOs with grouped items
            var orderDtos = new List<MyOrderDTO>();

            foreach (var order in orders.OrderByDescending(o => o.CreatedAt))
            {
                var orderDto = new MyOrderDTO
                {
                    Id = order.Id,
                    OrderStatus = order.OrderActivity != null && statusDictionary.ContainsKey(order.OrderActivity.Status)
                        ? statusDictionary[order.OrderActivity.Status]
                        : string.Empty,
                    TotalAmount = order.TotalAmount,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt,
                    AddressId = order.AddressId,
                    Items = new List<OrderItemWithDetailsDTO>()
                };

                foreach (var item in order.OrderItems)
                {
                    // Get item details from Catalogs module
                    var itemDetailsResult = await _mediator.Send(
                        new GetItemDetailsByBaseItemIdQuery(item.BaseItemId),
                        cancellationToken
                    );

                    if (!itemDetailsResult.Success)
                        continue;

                    var itemDetails = itemDetailsResult.Data;
                    string name = GetItemName(itemDetails);
                    string imageUrl = GetImageUrl(itemDetails);

                    orderDto.Items.Add(new OrderItemWithDetailsDTO
                    {
                        Name = name,
                        ImageUrl = imageUrl,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        TotalPrice = item.Price * item.Quantity
                    });
                }

                orderDtos.Add(orderDto);
            }

            // Apply pagination
            var paginatedOrders = orderDtos
                .Skip((request.Parameters.PageNumber - 1) * request.Parameters.PageSize)
                .Take(request.Parameters.PageSize)
                .ToList();

            var totalCount = orderDtos.Count;

            return Result<PaginatedResult<MyOrderDTO>>.Ok(
                data: PaginatedResult<MyOrderDTO>.Create(
                    paginatedOrders,
                    request.Parameters.PageNumber,
                    request.Parameters.PageSize,
                    totalCount),
                message: "تم جلب الطلبات بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders for user {UserId}", request.UserId);
            return Result<PaginatedResult<MyOrderDTO>>.Fail(
                message: "حدث خطأ أثناء جلب الطلبات",
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
            return product.Media.FirstOrDefault()?.Url ?? string.Empty;
        }
        else if (itemDetails is ServiceDetailsDTO service && service.Media != null && service.Media.Any())
        {
            return service.Media.FirstOrDefault()?.Url ?? string.Empty;
        }
        return string.Empty;
    }
} 