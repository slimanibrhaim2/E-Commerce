using Infrastructure.Common;
using Infrastructure.Models;
using Shoppings.Domain.Entities;

namespace Shoppings.Infrastructure.Mapping.Mappers
{
    public class OrderStatusMapper : BaseMapper<OrderStatusDAO, OrderStatus>
    {
        public override OrderStatus Map(OrderStatusDAO source)
        {
            return SafeMap(source, s => new OrderStatus
            {
                Id = s.Id,
                Name = s.Name,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                DeletedAt = s.DeletedAt
            });
        }

        public override OrderStatusDAO MapBack(OrderStatus target)
        {
            return SafeMapBack(target, t => new OrderStatusDAO
            {
                Id = t.Id,
                Name = t.Name,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DeletedAt = t.DeletedAt
            });
        }
    }
} 