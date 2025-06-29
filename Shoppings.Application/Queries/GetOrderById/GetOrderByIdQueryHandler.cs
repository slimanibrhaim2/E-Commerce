using MediatR;
using Core.Result;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Shoppings.Application.DTOs;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;
using System.Linq;

namespace Shoppings.Application.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderWithItemsDTO>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderStatusRepository _orderStatusRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<GetOrderByIdQueryHandler> _logger;

        public GetOrderByIdQueryHandler(
            IOrderRepository orderRepository,
            IOrderStatusRepository orderStatusRepository,
            IMediator mediator,
            ILogger<GetOrderByIdQueryHandler> logger)
        {
            _orderRepository = orderRepository;
            _orderStatusRepository = orderStatusRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Result<OrderWithItemsDTO>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderRepository.GetByIdWithItemsAsync(request.Id);
                if (order == null)
                {
                    _logger.LogWarning("Order not found with ID: {OrderId}", request.Id);
                    return Result<OrderWithItemsDTO>.Fail(
                        message: "الطلب غير موجود",
                        errorType: "OrderNotFound",
                        resultStatus: ResultStatus.NotFound);
                }

                // Get the status name
                var status = await _orderStatusRepository.GetByIdAsync(order.OrderActivity.Status);
                if (status == null)
                {
                    _logger.LogError("Status not found for ID: {StatusId}", order.OrderActivity.Status);
                    return Result<OrderWithItemsDTO>.Fail(
                        message: "حالة الطلب غير موجودة",
                        errorType: "StatusNotFound",
                        resultStatus: ResultStatus.Failed);
                }

                var orderDto = new OrderWithItemsDTO
                {
                    Id = order.Id,
                    StatusName = status.Name,
                    TotalAmount = order.TotalAmount,
                    Items = new List<OrderItemDetailsDTO>()
                };

                foreach (var item in order.OrderItems)
                {
                    // Get item details from Catalogs module
                    var itemDetailsQuery = new GetItemDetailsByBaseItemIdQuery(item.BaseItemId);
                    var itemDetailsResult = await _mediator.Send(itemDetailsQuery, cancellationToken);

                    if (!itemDetailsResult.Success)
                    {
                        _logger.LogWarning("Failed to get details for item {ItemId}", item.BaseItemId);
                        continue;
                    }

                    var itemDetails = itemDetailsResult.Data;
                    var orderItemDto = new OrderItemDetailsDTO
                    {
                        ItemId = itemDetails.Id, // Original product/service ID
                        ImageUrl = itemDetails.GetImageUrl(),
                        Name = itemDetails.GetName(),
                        Price = item.Price,
                        Quantity = item.Quantity,
                        TotalPrice = item.Price * item.Quantity
                    };

                    orderDto.Items.Add(orderItemDto);
                }

                return Result<OrderWithItemsDTO>.Ok(
                    data: orderDto,
                    message: "تم جلب تفاصيل الطلب بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order {OrderId}", request.Id);
                return Result<OrderWithItemsDTO>.Fail(
                    message: "فشل في جلب تفاصيل الطلب",
                    errorType: "GetOrderFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }


    }
} 