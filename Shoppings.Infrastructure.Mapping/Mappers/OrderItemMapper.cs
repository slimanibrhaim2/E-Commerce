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
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                DeletedAt = s.DeletedAt,
                Order = s.Order != null ? new Order 
                {
                    Id = s.Order.Id,
                    UserId = s.Order.UserId,
                    OrderActivityId = s.Order.OrderActivityId,
                    TotalAmount = s.Order.TotalAmount,
                    CreatedAt = s.Order.CreatedAt,
                    UpdatedAt = s.Order.UpdatedAt,
                    DeletedAt = s.Order.DeletedAt
                } : null
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
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DeletedAt = t.DeletedAt,
                Order = t.Order != null ? new OrderDAO
                {
                    Id = t.Order.Id,
                    UserId = t.Order.UserId,
                    OrderActivityId = t.Order.OrderActivityId,
                    TotalAmount = t.Order.TotalAmount,
                    CreatedAt = t.Order.CreatedAt,
                    UpdatedAt = t.Order.UpdatedAt,
                    DeletedAt = t.Order.DeletedAt
                } : null
            });
        }
    }
} 