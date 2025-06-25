using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Core.Result;
using Shoppings.Application.Commands.UpdateOrder;
using Core.Interfaces;
using System.Linq;

namespace Shoppings.Application.Commands.Handlers
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Result<bool>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.Id);
            if (order == null)
                return Result<bool>.Fail("Order not found.", "NotFound", ResultStatus.NotFound);
            order.OrderActivityId = request.OrderActivityId;
            order.TotalAmount = order.OrderItems.Sum(item => item.Quantity * item.Price);
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "updated", ResultStatus.Success);
        }
    }
} 