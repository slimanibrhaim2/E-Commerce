using Infrastructure.Common;
using Infrastructure.Models;
using Shoppings.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppings.Infrastructure.Mapping.Mappers
{
    public class CartMapper : BaseMapper<CartDAO, Cart>
    {
        public override Cart Map(CartDAO source)
        {
            return SafeMap(source, s => new Cart
            {
                Id = s.Id,
                UserId = s.UserId,
                CreatedAt = s.CreatedAt,
                DeletedAt = s.DeletedAt,
                UpdatedAt = s.UpdatedAt
            });
        }

        public override CartDAO MapBack(Cart target)
        {
            return SafeMapBack(target, t => new CartDAO
            {
                Id = t.Id,
                UserId = t.UserId,
                CreatedAt = t.CreatedAt,
                DeletedAt = t.DeletedAt,
                UpdatedAt = t.UpdatedAt
            });
        }
    }
}
