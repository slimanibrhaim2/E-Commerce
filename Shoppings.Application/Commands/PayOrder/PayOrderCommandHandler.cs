using Core.Result;
using MediatR;
using Microsoft.Extensions.Logging;
using Shoppings.Domain.Constants;
using Shoppings.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Shoppings.Domain.Entities;
using Shared.Contracts.Commands;

namespace Shoppings.Application.Commands.PayOrder
{
    public class PayOrderCommandHandler : IRequestHandler<PayOrderCommand, Result>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderStatusRepository _orderStatusRepository;
        private readonly ILogger<PayOrderCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderActivityRepository _orderActivityRepository;

        public PayOrderCommandHandler(
            IOrderRepository orderRepository,
            IOrderStatusRepository orderStatusRepository,
            ILogger<PayOrderCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IOrderActivityRepository orderActivityRepository)
        {
            _orderRepository = orderRepository;
            _orderStatusRepository = orderStatusRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _orderActivityRepository = orderActivityRepository;
        }

        public async Task<Result> Handle(PayOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get the order with its items
                var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId);
                if (order == null)
                {
                    return Result.Fail(
                        message: "لم يتم العثور على الطلب",
                        errorType: "OrderNotFound",
                        resultStatus: ResultStatus.NotFound);
                }

                // Get the paid status
                var statuses = await _orderStatusRepository.GetAllAsync();
                var paidStatus = statuses.FirstOrDefault(s => s.Name == OrderStatusNames.Paid);
                if (paidStatus == null)
                {
                    return Result.Fail(
                        message: "حالة الدفع غير موجودة",
                        errorType: "PaidStatusNotFound",
                        resultStatus: ResultStatus.Failed);
                }

                // Create new OrderActivity
                var newOrderActivity = new OrderActivity
                {
                    Id = Guid.NewGuid(),
                    Status = paidStatus.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Save new activity
                await _orderActivityRepository.AddAsync(newOrderActivity);

                // Update order with new activity
                order.OrderActivityId = newOrderActivity.Id;
                order.UpdatedAt = DateTime.UtcNow;

                // Save changes
                _orderRepository.Update(order);
                await _unitOfWork.SaveChangesAsync();

                return Result.Ok(
                    message: "تم تحديث حالة الطلب إلى مدفوع",
                    resultStatus: ResultStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking order {OrderId} as paid", request.OrderId);
                return Result.Fail(
                    message: "حدث خطأ أثناء تحديث حالة الطلب",
                    errorType: "PayOrderFailed",
                    resultStatus: ResultStatus.Failed,
                    exception: ex);
            }
        }
    }
} 