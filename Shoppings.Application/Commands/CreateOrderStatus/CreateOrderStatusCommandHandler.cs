using MediatR;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;

namespace Shoppings.Application.Commands.Handlers
{
    public class CreateOrderStatusCommandHandler : IRequestHandler<CreateOrderStatusCommand, OrderStatus>
    {
        private readonly IOrderStatusRepository _orderStatusRepository;
        public CreateOrderStatusCommandHandler(IOrderStatusRepository orderStatusRepository)
        {
            _orderStatusRepository = orderStatusRepository;
        }

        public async Task<OrderStatus> Handle(CreateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var entity = new OrderStatus { Name = request.Name };
            await _orderStatusRepository.AddAsync(entity);
            return entity;
        }
    }
} 