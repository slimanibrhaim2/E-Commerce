using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Core.Result;

namespace Shoppings.Application.Commands.Handlers
{
    public class UpdateOrderActivityCommandHandler : IRequestHandler<UpdateOrderActivityCommand, Result<bool>>
    {
        private readonly IOrderActivityRepository _orderActivityRepository;
        public UpdateOrderActivityCommandHandler(IOrderActivityRepository orderActivityRepository)
        {
            _orderActivityRepository = orderActivityRepository;
        }

        public async Task<Result<bool>> Handle(UpdateOrderActivityCommand request, CancellationToken cancellationToken)
        {
            var entity = await _orderActivityRepository.GetByIdAsync(request.Id);
            if (entity == null)
                return Result<bool>.Fail("OrderActivity not found.", "NotFound", ResultStatus.NotFound);
            entity.Status = request.Status;
            _orderActivityRepository.Update(entity);
            return Result<bool>.Ok(true, "updated", ResultStatus.Success);
        }
    }
} 