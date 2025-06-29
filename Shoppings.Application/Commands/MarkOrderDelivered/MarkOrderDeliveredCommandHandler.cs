using MediatR;
using Core.Result;
using Microsoft.Extensions.Logging;
using Shoppings.Domain.Repositories;
using Shoppings.Domain.Constants;
using System.Linq;
using Core.Interfaces;

namespace Shoppings.Application.Commands.MarkOrderDelivered;

public class MarkOrderDeliveredCommandHandler : IRequestHandler<MarkOrderDeliveredCommand, Result<bool>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderActivityRepository _orderActivityRepository;
    private readonly IOrderStatusRepository _orderStatusRepository;
    private readonly ILogger<MarkOrderDeliveredCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public MarkOrderDeliveredCommandHandler(
        IOrderRepository orderRepository,
        IOrderActivityRepository orderActivityRepository,
        IOrderStatusRepository orderStatusRepository,
        ILogger<MarkOrderDeliveredCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _orderActivityRepository = orderActivityRepository;
        _orderStatusRepository = orderStatusRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(MarkOrderDeliveredCommand request, CancellationToken cancellationToken)
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
                    message: "لا يمكنك تحديث حالة هذا الطلب لأنه لا ينتمي إليك",
                    errorType: "Unauthorized",
                    resultStatus: ResultStatus.Failed);
            }

            // Get the delivered status
            var deliveredStatuses = await _orderStatusRepository.FindAsync(s => s.Name == OrderStatusNames.Delivered);
            var deliveredStatus = deliveredStatuses.FirstOrDefault();
            if (deliveredStatus == null)
            {
                return Result<bool>.Fail(
                    message: "حالة التوصيل غير موجودة في النظام",
                    errorType: "NotFound",
                    resultStatus: ResultStatus.NotFound);
            }

            // Create new order activity with delivered status
            var orderActivity = new Domain.Entities.OrderActivity
            {
                Id = Guid.NewGuid(),
                Status = deliveredStatus.Id,
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

            return Result<bool>.Ok(
                data: true,
                message: "تم تحديث حالة الطلب إلى تم التوصيل بنجاح",
                resultStatus: ResultStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while marking order {OrderId} as delivered", request.OrderId);
            return Result<bool>.Fail(
                message: "حدث خطأ أثناء تحديث حالة الطلب",
                errorType: "ServerError",
                resultStatus: ResultStatus.Failed);
        }
    }
} 