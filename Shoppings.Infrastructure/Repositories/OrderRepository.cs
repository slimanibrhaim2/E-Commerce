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
    public class OrderRepository : BaseRepository<Order, OrderDAO>, IOrderRepository
    {
        private readonly ECommerceContext _ctx;
        private readonly IMapper<OrderDAO, Order> _orderMapper;
        public OrderRepository(ECommerceContext ctx, IMapper<OrderDAO, Order> mapper) : base(ctx, mapper)
        {
            _ctx = ctx;
            _orderMapper = mapper;
        }
    }
}
