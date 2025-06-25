using MediatR;
using Shoppings.Domain.Repositories;
using Core.Result;
using Core.Interfaces;

namespace Shoppings.Application.Commands.DeleteOrderItem
{
    public class DeleteOrderItemCommandHandler : IRequestHandler<DeleteOrderItemCommand, Result<bool>>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteOrderItemCommandHandler(IOrderItemRepository orderItemRepository, IUnitOfWork unitOfWork)
        {
            _orderItemRepository = orderItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteOrderItemCommand request, CancellationToken cancellationToken)
        {
            var orderItem = await _orderItemRepository.GetByIdAsync(request.Id);
            if (orderItem == null)
                return Result<bool>.Fail("عنصر الطلب غير موجود", "OrderItemNotFound", ResultStatus.NotFound);
            _orderItemRepository.Remove(orderItem);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "تم حذف العنصر من الطلب بنجاح", ResultStatus.Success);
        }
    }
} 