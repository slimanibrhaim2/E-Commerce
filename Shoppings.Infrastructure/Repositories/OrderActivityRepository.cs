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
    public class OrderActivityRepository : BaseRepository<OrderActivity, OrderActivityDAO>, IOrderActivityRepository
    {
        private readonly ECommerceContext _ctx;
        private readonly IMapper<OrderActivityDAO, OrderActivity> _orderActivityMapper;
        public OrderActivityRepository(ECommerceContext ctx, IMapper<OrderActivityDAO, OrderActivity> mapper) : base(ctx, mapper)
        {
            _ctx = ctx;
            _orderActivityMapper = mapper;
        }
    }
}
