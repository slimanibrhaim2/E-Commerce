using Communication.Domain.Entities;
using Communication.Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Communication.Infrastructure.Repositories
{
    public class ReviewRepository : BaseRepository<Review, ReviewDAO>, IReviewRepository
    {
        public ReviewRepository(ECommerceContext ctx, IMapper<ReviewDAO, Review> mapper)
            : base(ctx, mapper) { }

        public async Task<IEnumerable<Review>> GetAllByBaseItemIdAsync(Guid baseItemId)
        {
            var daos = await _dbSet.Where(r => r.BaseItemId == baseItemId && r.DeletedAt == null).ToListAsync();
            return daos.Select(d => _mapper.Map(d));
        }

        public async Task<IEnumerable<Review>> GetAllByUserIdAsync(Guid userId)
        {
            var daos = await _dbSet.Where(r => r.UserId == userId && r.DeletedAt == null).ToListAsync();
            return daos.Select(d => _mapper.Map(d));
        }

        public async Task<IEnumerable<Review>> GetAllByOrderIdAsync(Guid orderId)
        {
            var daos = await _dbSet.Where(r => r.OrderId == orderId && r.DeletedAt == null).ToListAsync();
            return daos.Select(d => _mapper.Map(d));
        }

        public async Task<Review?> GetByUserAndItemIdAsync(Guid userId, Guid baseItemId)
        {
            var dao = await _dbSet.FirstOrDefaultAsync(r => r.UserId == userId && r.BaseItemId == baseItemId && r.DeletedAt == null);
            return dao != null ? _mapper.Map(dao) : null;
        }

        public async Task<bool> HasUserReviewedItemAsync(Guid userId, Guid baseItemId)
        {
            return await _dbSet.AnyAsync(r => r.UserId == userId && r.BaseItemId == baseItemId && r.DeletedAt == null);
        }

        public async Task<bool> HasUserPurchasedItemAsync(Guid userId, Guid baseItemId, Guid orderId)
        {
            // Check if the user has an order item for this base item in the specified order
            return await _ctx.OrderItems
                .AnyAsync(oi => oi.Order.UserId == userId && 
                               oi.BaseItemId == baseItemId && 
                               oi.OrderId == orderId &&
                               oi.DeletedAt == null);
        }
    }
} 