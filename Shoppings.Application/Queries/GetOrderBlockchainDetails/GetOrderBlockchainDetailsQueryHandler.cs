using Core.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Contracts.DTOs.Blockchain;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;
using Shoppings.Domain.Repositories;

namespace Shoppings.Application.Queries.GetOrderBlockchainDetails;

public class GetOrderBlockchainDetailsQueryHandler : IRequestHandler<GetOrderBlockchainDetailsQuery, Result<OrderBlockchainDTO>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusRepository _orderStatusRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<GetOrderBlockchainDetailsQueryHandler> _logger;

    public GetOrderBlockchainDetailsQueryHandler(
        IOrderRepository orderRepository,
        IOrderStatusRepository orderStatusRepository,
        IMediator mediator,
        ILogger<GetOrderBlockchainDetailsQueryHandler> logger)
    {
        _orderRepository = orderRepository;
        _orderStatusRepository = orderStatusRepository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Result<OrderBlockchainDTO>> Handle(GetOrderBlockchainDetailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get order with items
            var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId);
            if (order == null)
            {
                return Result<OrderBlockchainDTO>.Fail(
                    message: "Order not found",
                    errorType: "OrderNotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // Get seller details using the Users module query
            var sellerResult = await _mediator.Send(new GetUserBlockchainInfoQuery(order.UserId), cancellationToken);
            if (!sellerResult.Success)
            {
                return Result<OrderBlockchainDTO>.Fail(
                    message: sellerResult.Message,
                    errorType: sellerResult.ErrorType,
                    resultStatus: sellerResult.ResultStatus);
            }

            // Get customer details using the Users module query
            var customerResult = await _mediator.Send(new GetUserBlockchainInfoQuery(order.UserId), cancellationToken);
            if (!customerResult.Success)
            {
                return Result<OrderBlockchainDTO>.Fail(
                    message: customerResult.Message,
                    errorType: customerResult.ErrorType,
                    resultStatus: customerResult.ResultStatus);
            }

            // Get the status name
            var status = await _orderStatusRepository.GetByIdAsync(order.OrderActivity.Status);
            if (status == null)
            {
                _logger.LogError("Status not found for ID: {StatusId}", order.OrderActivity.Status);
                return Result<OrderBlockchainDTO>.Fail(
                    message: "Order status not found",
                    errorType: "StatusNotFound",
                    resultStatus: ResultStatus.Failed);
            }

            // Build items list with detailed information
            var items = new List<OrderItemBlockchainDTO>();
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
                var orderItemDto = new OrderItemBlockchainDTO
                {
                    ItemId = itemDetails.Id,
                    ItemName = itemDetails.GetName(),
                    SerialNumber = itemDetails is ProductDetailsDTO productDetails ? productDetails.SerialNumber : null,
                    Quantity = Convert.ToInt32(item.Quantity),
                    UnitPrice = Convert.ToDouble(item.Price),
                    TotalPrice = Convert.ToDouble(item.Price * item.Quantity),
                };

                items.Add(orderItemDto);
            }

            // Map to DTO
            var orderBlockchainDTO = new OrderBlockchainDTO
            {
                OrderId = order.Id,
                OrderDate = order.CreatedAt,
                OrderStatus = status.Name,
                TotalAmount = Convert.ToDouble(order.TotalAmount),
                ShippingAddress = order.AddressId.ToString(),
                Items = items,
                Seller = sellerResult.Data,
                Customer = customerResult.Data
            };

            return Result<OrderBlockchainDTO>.Ok(
                data: orderBlockchainDTO,
                message: "Order blockchain details retrieved successfully",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving blockchain order details for order {OrderId}", request.OrderId);
            return Result<OrderBlockchainDTO>.Fail(
                message: "Error retrieving order details",
                errorType: "GetOrderDetailsFailed",
                resultStatus: ResultStatus.Failed);
        }
    }
} 