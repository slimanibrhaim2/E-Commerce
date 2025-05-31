using MediatR;
using Shoppings.Domain.Repositories;
using Core.Result;

namespace Shoppings.Application.Commands.DeleteOrderStatus
{
    public class DeleteOrderStatusCommandHandler : IRequestHandler<DeleteOrderStatusCommand, Result<bool>>
    {
        private readonly IOrderStatusRepository _orderStatusRepository;
        public DeleteOrderStatusCommandHandler(IOrderStatusRepository orderStatusRepository)
        {
            _orderStatusRepository = orderStatusRepository;
        }

        public async Task<Result<bool>> Handle(DeleteOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _orderStatusRepository.GetByIdAsync(request.Id);
            if (entity == null)
                return Result<bool>.Fail("OrderStatus not found.", "NotFound", ResultStatus.NotFound);
            _orderStatusRepository.Remove(entity);
            return Result<bool>.Ok(true, "deleted", ResultStatus.Success);
        }
    }
} 