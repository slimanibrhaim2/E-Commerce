using Infrastructure.Common;
using Infrastructure.Models;
using Shoppings.Domain.Entities;

namespace Shoppings.Infrastructure.Mapping.Mappers
{
    public class OrderActivityMapper : BaseMapper<OrderActivityDAO, OrderActivity>
    {
        public override OrderActivity Map(OrderActivityDAO source)
        {
            return SafeMap(source, s => new OrderActivity
            {
                Id = s.Id,
                Status = s.StatusId,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                DeletedAt = s.DeletedAt
            });
        }

        public override OrderActivityDAO MapBack(OrderActivity target)
        {
            return SafeMapBack(target, t => new OrderActivityDAO
            {
                Id = t.Id,
                StatusId = t.Status,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DeletedAt = t.DeletedAt
            });
        }
    }
} 