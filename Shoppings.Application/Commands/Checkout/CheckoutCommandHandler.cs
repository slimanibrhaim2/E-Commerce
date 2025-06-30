using MediatR;
using Shoppings.Domain.Repositories;
using Shoppings.Domain.Entities;
using Core.Result;
using Core.Interfaces;
using Shared.Contracts.Queries;
using Shared.Contracts.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Shared.Contracts.DTOs;

namespace Shoppings.Application.Commands.Checkout
{
    public class CheckoutCommandHandler : IRequestHandler<CheckoutCommand, Result<Guid>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IOrderActivityRepository _orderActivityRepository;
        private readonly IOrderStatusRepository _orderStatusRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ILogger<CheckoutCommandHandler> _logger;

        public CheckoutCommandHandler(
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            ICartItemRepository cartItemRepository,
            IOrderActivityRepository orderActivityRepository,
            IOrderStatusRepository orderStatusRepository,
            IUnitOfWork unitOfWork,
            IMediator mediator,
            ILogger<CheckoutCommandHandler> logger)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _cartItemRepository = cartItemRepository;
            _orderActivityRepository = orderActivityRepository;
            _orderStatusRepository = orderStatusRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Input validation
                if (request.CartId == Guid.Empty)
                {
                    return Result<Guid>.Fail(
                        message: "معرف سلة التسوق غير صالح",
                        errorType: "InvalidCartId",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (request.AddressId == Guid.Empty)
                {
                    return Result<Guid>.Fail(
                        message: "يجب اختيار عنوان للتوصيل",
                        errorType: "InvalidAddressId",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Start a transaction since we're making multiple changes
                await _unitOfWork.BeginTransaction();

                var cart = await _cartRepository.GetByIdAsync(request.CartId);
                if (cart == null)
                {
                    return Result<Guid>.Fail(
                        message: "لم يتم العثور على سلة التسوق",
                        errorType: "CartNotFound",
                        resultStatus: ResultStatus.NotFound);
                }

                if (cart.CartItems == null)
                {
                    return Result<Guid>.Fail(
                        message: "سلة التسوق غير صالحة",
                        errorType: "InvalidCart",
                        resultStatus: ResultStatus.ValidationError);
                }

                if (!cart.CartItems.Any())
                {
                    return Result<Guid>.Fail(
                        message: "لا يمكن إتمام الطلب. سلة التسوق فارغة",
                        errorType: "EmptyCart",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Check for deleted items
                if (cart.CartItems.Any(i => i.DeletedAt.HasValue))
                {
                    return Result<Guid>.Fail(
                        message: "يوجد منتجات محذوفة في سلة التسوق. يرجى إزالتها قبل إتمام الطلب",
                        errorType: "DeletedItemsInCart",
                        resultStatus: ResultStatus.ValidationError);
                }

                // Validate that all items are from the same seller
                Guid? firstSellerId = null;
                foreach (var cartItem in cart.CartItems)
                {
                    // First get the actual item ID from base item ID
                    var itemIdResult = await _mediator.Send(
                        new GetItemIdByBaseItemIdQuery(cartItem.BaseItemId),
                        cancellationToken
                    );

                    if (!itemIdResult.Success)
                    {
                        _logger.LogError("Failed to get item ID for base item {BaseItemId}", cartItem.BaseItemId);
                        return Result<Guid>.Fail(
                            message: "فشل في التحقق من معلومات المنتج",
                            errorType: "GetItemIdFailed",
                            resultStatus: ResultStatus.Failed);
                    }

                    // Then get the seller ID using the actual item ID
                    var userIdResult = await _mediator.Send(
                        new GetUserIdByItemIdQuery(itemIdResult.Data.ItemId),
                        cancellationToken
                    );

                    if (!userIdResult.Success)
                    {
                        _logger.LogError("Failed to get seller ID for item {ItemId}", itemIdResult.Data.ItemId);
                        return Result<Guid>.Fail(
                            message: "فشل في التحقق من معلومات البائع",
                            errorType: "GetSellerIdFailed",
                            resultStatus: ResultStatus.Failed);
                    }

                    if (firstSellerId == null)
                    {
                        firstSellerId = userIdResult.Data;
                    }
                    else if (firstSellerId != userIdResult.Data)
                    {
                        return Result<Guid>.Fail(
                            message: "لا يمكن إتمام الطلب. يجب أن تكون جميع المنتجات من نفس البائع. يمكنك إنشاء طلب منفصل لكل بائع",
                            errorType: "MultipleSellers",
                            resultStatus: ResultStatus.ValidationError);
                    }
                }
                
                // Get the pending status
                var allStatuses = await _orderStatusRepository.GetAllAsync();
                var pendingStatus = allStatuses.FirstOrDefault(s => s.Name == "قيد الانتظار");
                if (pendingStatus == null)
                {
                    _logger.LogError("Pending status not found in the database");
                    return Result<Guid>.Fail(
                        message: "حالة قيد الانتظار غير موجودة",
                        errorType: "PendingStatusNotFound",
                        resultStatus: ResultStatus.Failed);
                }

                // Create OrderActivity first
                var orderActivityId = Guid.NewGuid();
                var orderActivity = new OrderActivity
                {
                    Id = orderActivityId,
                    Status = pendingStatus.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _orderActivityRepository.AddAsync(orderActivity);

                var orderId = Guid.NewGuid();
                var order = new Order
                {
                    Id = orderId,
                    UserId = cart.UserId,
                    OrderActivityId = orderActivityId,
                    AddressId = request.AddressId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    TotalAmount = 0, // Will be calculated
                    OrderItems = new List<OrderItem>()
                };

                double total = 0;
                var orderItems = new List<OrderItem>();

                foreach (var cartItem in cart.CartItems)
                {
                    // Get item price and check if it's a product
                    var priceQuery = new GetItemPriceByIdQuery(cartItem.BaseItemId);
                    var priceResult = await _mediator.Send(priceQuery, cancellationToken);

                    if (!priceResult.Success)
                        return Result<Guid>.Fail(
                            message: $"فشل في جلب سعر العنصر {cartItem.BaseItemId}",
                            errorType: "GetPriceFailed",
                            resultStatus: ResultStatus.Failed);

                    // Try to get Item ID to update quantity
                    var itemQuery = new GetItemDetailsByBaseItemIdQuery(cartItem.BaseItemId);
                    var itemResult = await _mediator.Send(itemQuery, cancellationToken);

                    if (itemResult.Success)
                    {
                        // Check if it's a product
                        if (itemResult.Data is ProductDetailsDTO productDetails)
                        {
                            // Check if we have enough stock
                            if (productDetails.StockQuantity < cartItem.Quantity)
                            {
                                _logger.LogError("Insufficient stock for product {BaseItemId}. Required: {Required}, Available: {Available}", 
                                    cartItem.BaseItemId, cartItem.Quantity, productDetails.StockQuantity);
                                return Result<Guid>.Fail(
                                    message: "الكمية المطلوبة غير متوفرة في المخزون",
                                    errorType: "InsufficientStock",
                                    resultStatus: ResultStatus.ValidationError);
                            }

                            // Calculate new quantity by subtracting order quantity from current stock
                            var newQuantity = productDetails.StockQuantity - cartItem.Quantity;
                            var updateQuantityCommand = new UpdateProductQuantityCommand(productDetails.Id, newQuantity);
                            var updateResult = await _mediator.Send(updateQuantityCommand, cancellationToken);

                            if (!updateResult.Success)
                            {
                                _logger.LogError("Failed to update product {BaseItemId} quantity", cartItem.BaseItemId);
                                return Result<Guid>.Fail(
                                    message: "فشل في تحديث كمية المنتج",
                                    errorType: "UpdateQuantityFailed",
                                    resultStatus: ResultStatus.Failed);
                            }
                        }
                    }
                    else 
                    {
                        _logger.LogError("Failed to get item {BaseItemId} details", cartItem.BaseItemId);
                        return Result<Guid>.Fail(
                            message: "فشل في جلب تفاصيل المنتج",
                            errorType: "GetItemDetailsFailed",
                            resultStatus: ResultStatus.Failed);
                    }

                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderId,
                        BaseItemId = cartItem.BaseItemId,
                        Quantity = cartItem.Quantity,
                        Price = priceResult.Data.Price,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    orderItems.Add(orderItem);
                    total += orderItem.Price * orderItem.Quantity;

                    // Soft delete cart item
                    cartItem.DeletedAt = DateTime.UtcNow;
                    await _cartItemRepository.UpdateAsync(cartItem);
                }

                // Set the total amount before adding the order
                order.TotalAmount = total;
                order.OrderItems = orderItems;

                // Add the order with all its items
                await _orderRepository.AddAsync(order);

                // Save all changes in a single transaction
                await _unitOfWork.CommitTransaction();

                return Result<Guid>.Ok(
                    data: orderId,
                    message: "تم إنشاء الطلب بنجاح",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order from cart {CartId}", request.CartId);
                await _unitOfWork.RollbackTransaction();
                return Result<Guid>.Fail(
                    message: "فشل في تحويل سلة التسوق إلى طلب",
                    errorType: "TransactCartFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 