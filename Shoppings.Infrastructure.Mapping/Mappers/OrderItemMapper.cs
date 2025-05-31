using Infrastructure.Common;
using Infrastructure.Models;
using Shoppings.Domain.Entities;

namespace Shoppings.Infrastructure.Mapping.Mappers
{
    public class OrderItemMapper : BaseMapper<OrderItemDAO, OrderItem>
    {
        public override OrderItem Map(OrderItemDAO source)
        {
            return SafeMap(source, s => new OrderItem
            {
                Id = s.Id,
                OrderId = s.OrderId,
                BaseItemId = s.BaseItemId,
                Quantity = s.Quantity,
                Price = s.Price,
                CouponId = s.CouponId,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                DeletedAt = s.DeletedAt
            });
        }

        public override OrderItemDAO MapBack(OrderItem target)
        {
            return SafeMapBack(target, t => new OrderItemDAO
            {
                Id = t.Id,
                OrderId = t.OrderId,
                BaseItemId = t.BaseItemId,
                Quantity = t.Quantity,
                Price = t.Price,
                CouponId = t.CouponId,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DeletedAt = t.DeletedAt
            });
        }
    }
} 