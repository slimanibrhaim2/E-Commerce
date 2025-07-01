using Core.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using Shoppings.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;
using Shared.Contracts.Queries;
using Shared.Contracts.DTOs;

namespace Shoppings.Application.Queries.GetProviderIdByOrderId
{
    public class GetProviderIdByOrderIdQueryHandler : IRequestHandler<GetProviderIdByOrderIdQuery, Result<OrderProviderDTO>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetProviderIdByOrderIdQueryHandler> _logger;

        public GetProviderIdByOrderIdQueryHandler(
            IOrderRepository orderRepository,
            ILogger<GetProviderIdByOrderIdQueryHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Result<OrderProviderDTO>> Handle(GetProviderIdByOrderIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId);
                
                if (order == null)
                {
                    _logger.LogWarning("Order not found with ID: {OrderId}", request.OrderId);
                    return Result<OrderProviderDTO>.Fail(
                        message: "الطلب غير موجود",
                        errorType: "OrderNotFound",
                        resultStatus: ResultStatus.NotFound);
                }

                // Get the first item's provider ID
                var firstItem = order.OrderItems.FirstOrDefault();
                if (firstItem == null)
                {
                    _logger.LogWarning("Order {OrderId} has no items", request.OrderId);
                    return Result<OrderProviderDTO>.Fail(
                        message: "الطلب لا يحتوي على عناصر",
                        errorType: "OrderEmpty",
                        resultStatus: ResultStatus.NotFound);
                }

                // Get the user ID associated with this item (the provider)
                var providerId = await _orderRepository.GetProviderIdByOrderItemAsync(firstItem.Id);
                if (providerId == Guid.Empty)
                {
                    _logger.LogWarning("Provider not found for order item {OrderItemId}", firstItem.Id);
                    return Result<OrderProviderDTO>.Fail(
                        message: "لم يتم العثور على معرف المزود",
                        errorType: "ProviderNotFound",
                        resultStatus: ResultStatus.NotFound);
                }

                var dto = new OrderProviderDTO(providerId);

                return Result<OrderProviderDTO>.Ok(
                    data: dto,
                    message: "تم جلب معرف المزود بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting provider ID for order {OrderId}", request.OrderId);
                return Result<OrderProviderDTO>.Fail(
                    message: "حدث خطأ أثناء جلب معرف المزود",
                    errorType: "GetProviderIdFailed",
                    resultStatus: ResultStatus.Failed);
            }
        }
    }
} 