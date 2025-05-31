using MediatR;
using Shoppings.Domain.Repositories;
using Core.Result;
using Core.Interfaces;

namespace Shoppings.Application.Commands.Handlers
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Result<bool>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.Id);
            if (order == null)
                return Result<bool>.Fail("Order not found.", "NotFound", ResultStatus.NotFound);
            _orderRepository.Remove(order);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "deleted", ResultStatus.Success);
        }
    }
} 