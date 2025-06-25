using Core.Interfaces;
using Shoppings.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppings.Domain.Repositories
{
    public interface ICartItemRepository : IRepository<CartItem>
    {
        Task<CartItem?> GetByCartIdAndBaseItemIdAsync(Guid cartId, Guid baseItemId);
        Task<bool> UpdateAsync(CartItem cartItem);
    }
}
