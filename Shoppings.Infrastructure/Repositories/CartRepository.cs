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
    public class CartRepository : BaseRepository<Cart, CartDAO>, ICartRepository
    {
        private readonly ECommerceContext _ctx;
        private readonly IMapper<CartDAO, Cart> _cartMapper;
        
        public CartRepository(ECommerceContext ctx, IMapper<CartDAO, Cart> mapper) : base(ctx, mapper)
        {
            _ctx = ctx;
            _cartMapper = mapper;
        }

        public async Task<Cart?> GetActiveCartByUserIdAsync(Guid userId)
        {
            var cartDao = await _ctx.Carts
                .Where(c => c.UserId == userId && c.DeletedAt == null)
                .FirstOrDefaultAsync();
            
            return cartDao != null ? _cartMapper.Map(cartDao) : null;
        }

        public async Task<Cart> GetOrCreateCartByUserIdAsync(Guid userId)
        {
            var existingCart = await GetActiveCartByUserIdAsync(userId);
            if (existingCart != null)
            {
                return existingCart;
            }

            // Create new cart
            var newCart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await AddAsync(newCart);
            return newCart;
        }
    }
}
