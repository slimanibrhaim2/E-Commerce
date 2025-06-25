using MediatR;
using Shoppings.Domain.Repositories;
using Shoppings.Domain.Entities;
using Core.Result;
using Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Shoppings.Application.Commands.TransactCartToOrder
{
    public class TransactCartToOrderCommandHandler : IRequestHandler<TransactCartToOrderCommand, Result<Guid>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public TransactCartToOrderCommandHandler(
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(TransactCartToOrderCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(request.CartId);
            if (cart == null)
                return Result<Guid>.Fail("Cart not found.", "NotFound", ResultStatus.NotFound);
            if (cart.CartItems == null || !cart.CartItems.Any())
                return Result<Guid>.Fail("Cart is empty.", "ValidationError", ResultStatus.ValidationError);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = cart.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TotalAmount = 0 // Will be calculated
            };
            await _orderRepository.AddAsync(order);

            double total = 0;
            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    BaseItemId = cartItem.BaseItemId,
                    Quantity = cartItem.Quantity,
                    Price = 0, // Should be set by pricing logic
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _orderItemRepository.AddAsync(orderItem);
                total += orderItem.Price * orderItem.Quantity; // Uncomment when pricing is set
            }
            order.TotalAmount = total;
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();
            return Result<Guid>.Ok(order.Id, "Order created from cart.", ResultStatus.Success);
        }
    }
} 