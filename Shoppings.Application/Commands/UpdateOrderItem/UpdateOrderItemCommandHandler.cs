using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Core.Result;
using Core.Interfaces;

namespace Shoppings.Application.Commands.UpdateOrderItem
{
    public class UpdateOrderItemCommandHandler : IRequestHandler<UpdateOrderItemCommand, Result<bool>>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateOrderItemCommandHandler(IOrderItemRepository orderItemRepository, IUnitOfWork unitOfWork)
        {
            _orderItemRepository = orderItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateOrderItemCommand request, CancellationToken cancellationToken)
        {
            var orderItem = await _orderItemRepository.GetByIdAsync(request.Id);
            if (orderItem == null)
                return Result<bool>.Fail("OrderItem not found.", "NotFound", ResultStatus.NotFound);
            orderItem.Quantity = request.Quantity;
            orderItem.Price = request.Price;
            _orderItemRepository.Update(orderItem);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "updated", ResultStatus.Success);
        }
    }
} 