using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Core.Result;
using Shoppings.Application.Commands.CreateOrder;
using Core.Interfaces;

namespace Shoppings.Application.Commands.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty || request.OrderActivityId == Guid.Empty)
                return Result<Guid>.Fail("معرف المستخدم ومعرف نشاط الطلب مطلوبان", "ValidationError", ResultStatus.ValidationError);
            var order = new Order
            {
                UserId = request.UserId,
                OrderActivityId = request.OrderActivityId,
                TotalAmount = 0 // Will be updated after items are added
            };
            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
            return Result<Guid>.Ok(order.Id, "تم إنشاء الطلب بنجاح", ResultStatus.Success);
        }
    }
} 