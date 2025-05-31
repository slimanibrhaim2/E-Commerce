using Infrastructure.Common;
using Infrastructure.Models;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoppings.Infrastructure.Repositories
{
    public class CartItemRepository : BaseRepository<CartItem, CartItemDAO>, ICartItemRepository
    {
        private readonly ECommerceContext _ctx;
        private readonly IMapper<CartItemDAO, CartItem> _cartMapper;
        public CartItemRepository(ECommerceContext ctx, IMapper<CartItemDAO, CartItem> mapper) : base(ctx, mapper)
        {
            _ctx = ctx;
            _cartMapper= mapper;
        }
    }
}
