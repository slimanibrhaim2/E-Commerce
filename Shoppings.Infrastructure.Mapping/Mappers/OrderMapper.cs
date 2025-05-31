using Infrastructure.Common;
using Infrastructure.Models;
using Shoppings.Domain.Entities;

namespace Shoppings.Infrastructure.Mapping.Mappers
{
    public class OrderMapper : BaseMapper<OrderDAO, Order>
    {
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
                DeletedAt = s.DeletedAt
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
                DeletedAt = t.DeletedAt
            });
        }
    }
} 