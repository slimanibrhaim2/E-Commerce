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
    public class OrderItemRepository : BaseRepository<OrderItem, OrderItemDAO>, IOrderItemRepository
    {
        private readonly ECommerceContext _ctx;
        private readonly IMapper<OrderItemDAO, OrderItem> _orderItemMapper;
        public OrderItemRepository(ECommerceContext ctx, IMapper<OrderItemDAO, OrderItem> mapper) : base(ctx, mapper)
        {
            _ctx = ctx;
            _orderItemMapper = mapper;
        }
    }
}
