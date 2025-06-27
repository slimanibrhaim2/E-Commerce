using Infrastructure.Common;
using Infrastructure.Models;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Shoppings.Infrastructure.Repositories
{
    public class CartItemRepository : BaseRepository<CartItem, CartItemDAO>, ICartItemRepository
    {
        private readonly ECommerceContext _ctx;
        private readonly IMapper<CartItemDAO, CartItem> _cartMapper;
        
        public CartItemRepository(ECommerceContext ctx, IMapper<CartItemDAO, CartItem> mapper) : base(ctx, mapper)
        {
            _ctx = ctx;
            _cartMapper = mapper;
        }

        public async Task<CartItem?> GetByCartIdAndBaseItemIdAsync(Guid cartId, Guid baseItemId)
        {
            var cartItemDao = await _ctx.CartItems
                .Include(ci => ci.BaseItem)
                .Where(ci => ci.CartId == cartId && ci.BaseItemId == baseItemId && ci.DeletedAt == null)
                .FirstOrDefaultAsync();
            
            return cartItemDao != null ? _cartMapper.Map(cartItemDao) : null;
        }

        public async Task<bool> UpdateAsync(CartItem cartItem)
        {
            try
            {
                var existingDao = await _ctx.CartItems
                    .Include(ci => ci.BaseItem)
                    .FirstOrDefaultAsync(ci => ci.Id == cartItem.Id && ci.DeletedAt == null);
                
                if (existingDao == null)
                    return false;

                // Update properties
                existingDao.Quantity = cartItem.Quantity;
                existingDao.UpdatedAt = cartItem.UpdatedAt;
                existingDao.DeletedAt = cartItem.DeletedAt;

                // EF will track changes automatically - SaveChanges is handled by UnitOfWork
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
