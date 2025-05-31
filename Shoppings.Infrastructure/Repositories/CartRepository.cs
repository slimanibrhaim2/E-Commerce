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
    public class CartRepository : BaseRepository<Cart, CartDAO>, ICartRepository
    {
        private readonly ECommerceContext _ctx;
        private readonly IMapper<CartDAO, Cart> _cartItemMapper;
        public CartRepository(ECommerceContext ctx, IMapper<CartDAO, Cart> mapper) : base(ctx, mapper)
        {
            _ctx = ctx;
            _cartItemMapper = mapper;
        }
    }
}
