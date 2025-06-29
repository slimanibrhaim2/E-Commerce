using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Core.Result;
using Shoppings.Application.Commands.CreateOrderItem;
using Core.Interfaces;

namespace Shoppings.Application.Commands.Handlers
{
    public class CreateOrderItemCommandHandler : IRequestHandler<CreateOrderItemCommand, Result<Guid>>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateOrderItemCommandHandler(IOrderItemRepository orderItemRepository, IUnitOfWork unitOfWork)
        {
            _orderItemRepository = orderItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateOrderItemCommand request, CancellationToken cancellationToken)
        {
            if (request.OrderId == Guid.Empty || request.BaseItemId == Guid.Empty)
                return Result<Guid>.Fail("معرف الطلب ومعرف العنصر مطلوبان", "ValidationError", ResultStatus.ValidationError);
            if (request.Quantity <= 0)
                return Result<Guid>.Fail("يجب أن تكون الكمية أكبر من صفر", "ValidationError", ResultStatus.ValidationError);
            var orderItem = new OrderItem
            {
                OrderId = request.OrderId,
                BaseItemId = request.BaseItemId,
                Quantity = request.Quantity,
                Price = request.Price
            };
            await _orderItemRepository.AddAsync(orderItem);
            await _unitOfWork.SaveChangesAsync();
            return Result<Guid>.Ok(orderItem.Id, "تم إضافة العنصر إلى الطلب بنجاح", ResultStatus.Success);
        }
    }
} 