using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Core.Result;

namespace Shoppings.Application.Commands.Handlers
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Result<bool>>
    {
        private readonly IOrderStatusRepository _orderStatusRepository;
        public UpdateOrderStatusCommandHandler(IOrderStatusRepository orderStatusRepository)
        {
            _orderStatusRepository = orderStatusRepository;
        }

        public async Task<Result<bool>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = await _orderStatusRepository.GetByIdAsync(request.Id);
            if (entity == null)
                return Result<bool>.Fail("OrderStatus not found.", "NotFound", ResultStatus.NotFound);
            entity.Name = request.Name;
            _orderStatusRepository.Update(entity);
            return Result<bool>.Ok(true, "updated", ResultStatus.Success);
        }
    }
} 