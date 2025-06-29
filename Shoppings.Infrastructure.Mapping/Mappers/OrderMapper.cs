using Infrastructure.Common;
using Infrastructure.Models;
using Shoppings.Domain.Entities;

namespace Shoppings.Infrastructure.Mapping.Mappers
{
    public class OrderMapper : BaseMapper<OrderDAO, Order>
    {
        private readonly IMapper<OrderItemDAO, OrderItem> _orderItemMapper;
        private readonly IMapper<OrderActivityDAO, OrderActivity> _orderActivityMapper;

        public OrderMapper(
            IMapper<OrderItemDAO, OrderItem> orderItemMapper,
            IMapper<OrderActivityDAO, OrderActivity> orderActivityMapper)
        {
            _orderItemMapper = orderItemMapper;
            _orderActivityMapper = orderActivityMapper;
        }

        public override Order Map(OrderDAO source)
        {
            return SafeMap(source, s => new Order
            {
                Id = s.Id,
                UserId = s.UserId,
                OrderActivityId = s.OrderActivityId,
                TotalAmount = s.TotalAmount,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                DeletedAt = s.DeletedAt,
                OrderItems = s.OrderItems?.Select(oi => _orderItemMapper.Map(oi)).ToList(),
                OrderActivity = s.OrderActivity != null ? _orderActivityMapper.Map(s.OrderActivity) : null
            });
        }

        public override OrderDAO MapBack(Order target)
        {
            return SafeMapBack(target, t => new OrderDAO
            {
                Id = t.Id,
                UserId = t.UserId,
                OrderActivityId = t.OrderActivityId,
                TotalAmount = t.TotalAmount,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DeletedAt = t.DeletedAt,
                OrderItems = t.OrderItems?.Select(oi => _orderItemMapper.MapBack(oi)).ToList(),
                OrderActivity = t.OrderActivity != null ? _orderActivityMapper.MapBack(t.OrderActivity) : null
            });
        }
    }
} 