using Infrastructure.Common;
using Infrastructure.Models;
using Shoppings.Domain.Entities;

namespace Shoppings.Infrastructure.Mapping.Mappers
{
    public class CartItemMapper : BaseMapper<CartItemDAO, CartItem>
    {
        public override CartItem Map(CartItemDAO source)
        {
            return SafeMap(source, s => new CartItem
            {
                Id = s.Id,
                CartId = s.CartId,
                BaseItemId = s.BaseItemId,
                Quantity = s.Quantity,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                DeletedAt = s.DeletedAt
            });
        }

        public override CartItemDAO MapBack(CartItem target)
        {
            return SafeMapBack(target, t => new CartItemDAO
            {
                Id = t.Id,
                CartId = t.CartId,
                BaseItemId = t.BaseItemId,
                Quantity = t.Quantity,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DeletedAt = t.DeletedAt
            });
        }
    }
} 