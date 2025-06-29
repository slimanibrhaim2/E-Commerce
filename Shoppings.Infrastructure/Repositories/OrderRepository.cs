using Infrastructure.Common;
using Infrastructure.Models;
using Shoppings.Domain.Entities;
using Shoppings.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Order?> GetByIdWithItemsAsync(Guid id)
        {
            var orderDao = await _ctx.Orders
                .Include(o => o.OrderItems.Where(oi => oi.DeletedAt == null))
                .Include(o => o.OrderActivity)
                .FirstOrDefaultAsync(o => o.Id == id && o.DeletedAt == null);

            return orderDao != null ? _orderMapper.Map(orderDao) : null;
        }

        public async Task<IEnumerable<Order>> GetAllWithItemsAsync()
        {
            var orderDaos = await _ctx.Orders
                .Include(o => o.OrderItems.Where(oi => oi.DeletedAt == null))
                .Include(o => o.OrderActivity)
                .Where(o => o.DeletedAt == null)
                .ToListAsync();

            return orderDaos.Select(dao => _orderMapper.Map(dao));
        }

        public async Task<IEnumerable<Order>> GetAllByUserIdWithItemsAsync(Guid userId)
        {
            var orderDaos = await _ctx.Orders
                .Include(o => o.OrderItems.Where(oi => oi.DeletedAt == null))
                .Include(o => o.OrderActivity)
                .Where(o => o.UserId == userId && o.DeletedAt == null)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orderDaos.Select(dao => _orderMapper.Map(dao));
        }
    }
}
