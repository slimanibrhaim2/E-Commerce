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
    public class OrderStatusRepository : BaseRepository<OrderStatus, OrderStatusDAO>, IOrderStatusRepository
    {
        private readonly ECommerceContext _ctx;
        private readonly IMapper<OrderStatusDAO, OrderStatus> _orderStatusMapper;
        public OrderStatusRepository(ECommerceContext ctx, IMapper<OrderStatusDAO, OrderStatus> mapper) : base(ctx, mapper)
        {
            _ctx = ctx;
            _orderStatusMapper = mapper;
        }
    }
}
