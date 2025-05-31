using MediatR;
using Shoppings.Domain.Repositories;
using Core.Result;

namespace Shoppings.Application.Commands.Handlers
{
    public class DeleteOrderActivityCommandHandler : IRequestHandler<DeleteOrderActivityCommand, Result<bool>>
    {
        private readonly IOrderActivityRepository _orderActivityRepository;
        public DeleteOrderActivityCommandHandler(IOrderActivityRepository orderActivityRepository)
        {
            _orderActivityRepository = orderActivityRepository;
        }

        public async Task<Result<bool>> Handle(DeleteOrderActivityCommand request, CancellationToken cancellationToken)
        {
            var entity = await _orderActivityRepository.GetByIdAsync(request.Id);
            if (entity == null)
                return Result<bool>.Fail("OrderActivity not found.", "NotFound", ResultStatus.NotFound);
            _orderActivityRepository.Remove(entity);
            return Result<bool>.Ok(true, "deleted", ResultStatus.Success);
        }
    }
} 