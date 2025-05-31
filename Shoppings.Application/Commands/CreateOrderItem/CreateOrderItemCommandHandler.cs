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
                return Result<Guid>.Fail("OrderId and BaseItemId are required.", "ValidationError", ResultStatus.ValidationError);
            if (request.Quantity <= 0)
                return Result<Guid>.Fail("Quantity must be greater than zero.", "ValidationError", ResultStatus.ValidationError);
            var orderItem = new OrderItem
            {
                OrderId = request.OrderId,
                BaseItemId = request.BaseItemId,
                Quantity = request.Quantity,
                Price = request.Price,
                CouponId = request.CouponId
            };
            await _orderItemRepository.AddAsync(orderItem);
            await _unitOfWork.SaveChangesAsync();
            return Result<Guid>.Ok(orderItem.Id,"added" , ResultStatus.Success);
        }
    }
} 