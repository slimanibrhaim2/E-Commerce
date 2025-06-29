using MediatR;
using Core.Result;
using Microsoft.Extensions.Logging;
using Shoppings.Domain.Repositories;
using Shared.Contracts.Commands;
using Shoppings.Domain.Constants;
using System.Linq;
using Core.Interfaces;

namespace Shoppings.Application.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<bool>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderActivityRepository _orderActivityRepository;
    private readonly IOrderStatusRepository _orderStatusRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<CancelOrderCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(
        IOrderRepository orderRepository,
        IOrderActivityRepository orderActivityRepository,
        IOrderStatusRepository orderStatusRepository,
        IMediator mediator,
        ILogger<CancelOrderCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _orderActivityRepository = orderActivityRepository;
        _orderStatusRepository = orderStatusRepository;
        _mediator = mediator;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get the order with its items
            var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId);
            if (order == null)
            {
                return Result<bool>.Fail(
                    message: "الطلب غير موجود",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // Check if the user owns this order
            if (order.UserId != request.UserId)
            {
                return Result<bool>.Fail(
                    message: "لا يمكنك إلغاء هذا الطلب لأنه لا ينتمي إليك",
                    errorType: "Unauthorized",
                    resultStatus: ResultStatus.Failed);
            }

            // Get the cancelled status
            var cancelledStatuses = await _orderStatusRepository.FindAsync(s => s.Name == OrderStatusNames.Cancelled);
            var cancelledStatus = cancelledStatuses.FirstOrDefault();
            if (cancelledStatus == null)
            {
                return Result<bool>.Fail(
                    message: "حالة الإلغاء غير موجودة في النظام",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // Create new order activity with cancelled status
            var orderActivity = new Domain.Entities.OrderActivity
            {
                Id = Guid.NewGuid(),
                Status = cancelledStatus.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Save the new activity
            await _orderActivityRepository.AddAsync(orderActivity);

            // Update order with new activity
            order.OrderActivityId = orderActivity.Id;
            order.UpdatedAt = DateTime.UtcNow;
            _orderRepository.Update(order);

            // Save all changes
            await _unitOfWork.SaveChangesAsync();

            // Return quantities to products using the shared contract
            foreach (var item in order.OrderItems)
            {
                var updateQuantityResult = await _mediator.Send(
                    new UpdateProductQuantityCommand(
                        ProductId: item.Id,
                        QuantityChange: item.Quantity // Add back the quantity that was reserved
                    ),
                    cancellationToken
                );

                if (!updateQuantityResult.Success)
                {
                    _logger.LogWarning(
                        "Failed to return quantity for product {ProductId} in order {OrderId}: {Message}",
                        item.Id,
                        order.Id,
                        updateQuantityResult.Message
                    );
                }
            }

            return Result<bool>.Ok(
                data: true,
                message: "تم إلغاء الطلب بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cancelling order {OrderId}", request.OrderId);
            return Result<bool>.Fail(
                message: "حدث خطأ أثناء إلغاء الطلب",
                errorType: "ServerError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 