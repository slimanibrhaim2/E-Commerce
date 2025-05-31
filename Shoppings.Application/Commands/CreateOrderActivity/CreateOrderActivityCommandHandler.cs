using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;

namespace Shoppings.Application.Commands.Handlers
{
    public class CreateOrderActivityCommandHandler : IRequestHandler<CreateOrderActivityCommand, OrderActivity>
    {
        private readonly IOrderActivityRepository _orderActivityRepository;
        public CreateOrderActivityCommandHandler(IOrderActivityRepository orderActivityRepository)
        {
            _orderActivityRepository = orderActivityRepository;
        }

        public async Task<OrderActivity> Handle(CreateOrderActivityCommand request, CancellationToken cancellationToken)
        {
            var entity = new OrderActivity { Status = request.Status };
            await _orderActivityRepository.AddAsync(entity);
            return entity;
        }
    }
} 